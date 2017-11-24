using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileInfo = Pri.LongPath.FileInfo;
using DirectoryInfo = Pri.LongPath.DirectoryInfo;
using File = Pri.LongPath.File;
using Path = Pri.LongPath.Path;
using SimpleImpersonation;
using System.Security;
using MimeTypes;
using Ionic.Zip;
using System.IO;
using System.Text.RegularExpressions;

namespace Hornet.IO
{
    /// <summary>
    /// Static object used for governing scans.
    /// </summary>
    public static class HornetScanManager
    {
        //Private hashsets containing just the hashes for lookup performance
        private static HashSet<string> _md5HashSet = new HashSet<string>();
        private static HashSet<string> _sha1HashSet = new HashSet<string>();
        private static HashSet<string> _sha256HashSet = new HashSet<string>();

        //Private list of regexinfos to make multi threaded checking easier, so
        //no need to lock the entire results group for enumeration and adding,
        //has the references set to those same as groups for easy lookup later
        private static List<RegexInfo> _regexInfos = new List<RegexInfo>();

        //Private flags for convenience instead of constantly checking the
        //hashset counts
        private static bool _includeMD5 = false;
        private static bool _includeSHA1 = false;
        private static bool _includeSHA256 = false;
        private static bool _includeRegex = false;

        private static ScanOptions _options;

        //Some of the options are copied to here on start scan just
        //for the sake of concise syntax
        private static int _fileQueueLimit;

        //Working queues used to avoid enormous amounts of recursion on SAN
        //scale file systems
        private static ConcurrentStack<string> _directoryStack = new ConcurrentStack<string>();
        private static ConcurrentStack<string> _backgroundDirectoryStack = new ConcurrentStack<string>();
        private static ConcurrentQueue<string> _fileWorkingQueue = new ConcurrentQueue<string>();

        public static int CurrentQueueSize { get { return _fileWorkingQueue.Count; } }

        //Flags for maintaining state with all the different threads.
        private static bool _backgroundEnumerationFinished = false;
        private static bool _directoryEnumerationFinished = false;

        private static List<Thread> _workerThreads = new List<Thread>();

        //Used when credentials are provided so the scan manager will operate within
        //this impersonation context
        private static Impersonation _impersonationContext;

        /// <summary>
        /// Gets the current status of the scan as a <see cref="ScanStatus"/>
        /// </summary>
        public static ScanStatus Status { get; private set; } = new ScanStatus();

        /// <summary>
        /// Gets the results of the scan as a <see cref="ScanResult"/>
        /// </summary>
        public static ScanResult Results { get; private set; } = new ScanResult();



        /// <summary>
        /// Starts a scan running with the options provided.
        /// </summary>
        /// <param name="options">The options to run the scan with</param>
        public static async void StartScanAsync(ScanOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("Options must be specified starting a scan");
            }

            _options = options;
            _fileQueueLimit = _options.MaxFileQueueSize;

            AddNewScanEvent(ScanEventType.Start, DateTime.Now);

            BeginImpersonation();

            Status.ScanRunning = true;
            Status.Message = "Preparing internal data...";

            await Task.Run(() =>
            {
                
                AddHashesToInternalSets();

                AddRegexToInternalList();

                AddGroupsToResultProperty();

                Status.Message = "Commencing worker threads...";

                if (!TryInitialEnumeration())
                {
                    Status.ScanRunning = false;
                    Status.ScanFinished = true;
                    Status.Message = "Failed initial enumeration";
                    AddNewScanEvent(ScanEventType.Finish, DateTime.Now);
                    return;
                }

                if (_options.BackgroundEnumerate) StartBackgroundEnumeration();

                StartDirectoryEnumeration();

                StartFileWorkers();
            });
        }

        

        private static void StartFileWorkers()
        {
            int threadCount = _options.MaxWorkerThreads < 1 ? 1 : _options.MaxWorkerThreads;

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(DoWork);
                _workerThreads.Add(thread);
                thread.Start();
            }

            Thread watcherThread = new Thread(() =>
            {
                while (_workerThreads.Count(t=>t.IsAlive) > 0)
                {
                    Thread.Sleep(1000);
                }

                Status.Message = "Finished";
                Status.ScanFinished = true;
                Status.ScanRunning = false;
                AddNewScanEvent(ScanEventType.Finish, DateTime.Now);
            });

            watcherThread.Start();
        }

        private static void DoWork()
        {
            while (_directoryEnumerationFinished == false || _fileWorkingQueue.Count > 0)
            {
                string filePath;
                if (_fileWorkingQueue.TryDequeue(out filePath))
                {
                    FileReader fileReader = new FileReader(filePath, _options, _includeMD5, _includeSHA1, _includeSHA256, _includeRegex);
                    FileResult fileResult = fileReader.GetResult();

                    LogFileCountUpdate(fileResult);

                    if (fileResult.ResultType == ResultType.Read) CheckResult(fileResult);
                }
            }
        }

        private static void LogFileCountUpdate(FileResult fileResult)
        {
            LogHashCounts(fileResult);

            switch (fileResult.ResultType)
            {
                case ResultType.Skipped:
                    Interlocked.Increment(ref Status.TotalFilesSkipped);
                    break;

                case ResultType.Failed:
                    Interlocked.Increment(ref Status.TotalFilesFailed);
                    break;

                case ResultType.Read:
                    Interlocked.Add(ref Status.TotalBytesProcessed, fileResult.Length);
                    Interlocked.Increment(ref Status.TotalFilesSucceeded);
                    Interlocked.Increment(ref Status.TotalBytesProcessed);
                    break;

                case ResultType.Encrypted:
                    Interlocked.Increment(ref Status.TotalFilesEncrypted);
                    if (_options.ListEncryptedFiles)
                    {
                        Results.AddEncryptedFile(fileResult.Name);
                    }
                    break;
            }

            
        }

        private static void LogHashCounts(FileResult fileResult)
        {
            if (fileResult.MD5 != null) Interlocked.Increment(ref Status.FileHashesPerformed);
            if (fileResult.SHA1 != null) Interlocked.Increment(ref Status.FileHashesPerformed);
            if (fileResult.SHA256 != null) Interlocked.Increment(ref Status.FileHashesPerformed);

            foreach (FileResult embeddedResult in fileResult.EmbeddedResults)
            {
                LogHashCounts(embeddedResult);
            }
        }

        private static void CheckResult(FileResult fileResult)
        {
            if (IsHashMatch(fileResult))
            {
                AddToHashResults(fileResult);
            }

            List<Tuple<RegexInfo,string>> matchedRegexInfos = new List<Tuple<RegexInfo, string>>();
            if (IsRegexMatch(fileResult, matchedRegexInfos))
            {
                AddToRegexMatch(fileResult, matchedRegexInfos);
            }
        }

        private static void AddToRegexMatch(FileResult fileResult, IEnumerable<Tuple<RegexInfo,string>> matches)
        {
            lock (Results.RegexGroups)
            {
                foreach (RegexInfoGroup group in Results.RegexGroups)
                {
                    RegexResult result = new RegexResult()
                    {
                        Length = fileResult.Length,
                        Name = fileResult.Name,
                        MimeType = MimeTypeMap.GetMimeType(Path.GetExtension(fileResult.Name) ?? string.Empty) 
                    };

                    List<Tuple<RegexInfo, string>> matchedRegexInfos = new List<Tuple<RegexInfo, string>>();

                    foreach (var match in matches)
                    {
                        if (group.RegexInfos.Contains(match.Item1))
                        {
                            matchedRegexInfos.Add(match);
                        }
                    }

                    result.MatchedRegexInfos = matchedRegexInfos;

                    group.Matches.Add(result);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the content of the file matched any
        /// regex's specified.  If matches then matchedInfos out parameter will be populated
        /// with <see cref="Tuple{T1, T2}"/>, with <see cref="T1"/> being the matched
        /// <see cref="RegexInfo"/> and <see cref="T2"/> being the matched string
        /// and a preview of text surrounding the match (if available)
        /// </summary>
        /// <param name="fileResult"></param>
        /// <param name="matchedInfos"></param>
        /// <returns></returns>
        private static bool IsRegexMatch(FileResult fileResult, List<Tuple<RegexInfo, string>> matchedInfos)
        {
            if (string.IsNullOrWhiteSpace(fileResult.Content) && fileResult.EmbeddedResults.Count == 0)
            {
                return false;
            }

            if (matchedInfos == null) matchedInfos = new List<Tuple<RegexInfo, string>>();
            foreach (RegexInfo regexInfo in _regexInfos)
            {
                MatchCollection matches = regexInfo.AsRegex().Matches(fileResult.Content);

                for (int i = 0; i < matches.Count; i++)
                {
                    int startPreviewIndex = 0;
                    if (matches[i].Index > 25) startPreviewIndex = matches[i].Index - 25;

                    int endPreviewIndex = fileResult.Content.Length - 1;
                    if (matches[i].Index + matches[i].Length + 25 < endPreviewIndex) endPreviewIndex = matches[i].Index + matches[i].Length + 25;

                    string previewText = fileResult.Content.Substring(startPreviewIndex, endPreviewIndex - startPreviewIndex);
                    matchedInfos.Add(new Tuple<RegexInfo, string>(regexInfo, previewText));
                }
            }

            foreach (FileResult embeddedResult in fileResult.EmbeddedResults)
            {
                IsRegexMatch(embeddedResult, matchedInfos);
            }

            return matchedInfos.Count > 0;
        }

        private static void AddToHashResults(FileResult fileResult)
        {
            lock (Results.HashGroups)
            {
                foreach (HashInfoGroup group in Results.HashGroups)
                {
                    List<HashInfo> matchedHashInfos = new List<HashInfo>();
                    if (fileResult.MD5 != null) matchedHashInfos.AddRange(group.MD5s.Where(h => h.Hash == fileResult.MD5));
                    if (fileResult.SHA1 != null) matchedHashInfos.AddRange(group.SHA1s.Where(h => h.Hash == fileResult.SHA1));
                    if (fileResult.SHA256 != null) matchedHashInfos.AddRange(group.SHA256s.Where(h => h.Hash == fileResult.SHA256));

                    if (matchedHashInfos.Count == 0) continue; //only add to the groups where it belongs

                    HashResult hashResult = new HashResult()
                    {
                        Length = fileResult.Length,
                        MimeType = MimeTypeMap.GetMimeType(Path.GetExtension(fileResult.Name) ?? string.Empty),
                        MatchedHashInfos = matchedHashInfos,
                        Name = fileResult.Name
                    };

                    group.Matches.Add(hashResult);
                }
            }
            
        }

        private static bool IsHashMatch(FileResult fileResult)
        {
            if (fileResult.MD5 != null && _md5HashSet.Contains(fileResult.MD5)) return true;
            if (fileResult.SHA1 != null && _sha1HashSet.Contains(fileResult.SHA1)) return true;
            if (fileResult.SHA256 != null && _sha256HashSet.Contains(fileResult.SHA256)) return true;

            foreach (FileResult embeddedResult in fileResult.EmbeddedResults)
            {
                if (IsHashMatch(embeddedResult)) return true;
            }

            return false;
        }

        private static void StartDirectoryEnumeration()
        {
            //TODO: no plans to multi thread this actually, so does it
            //need to be using a concurrent stack?... need to check where else
            //I've accessed it
            _directoryEnumerationFinished = false;

            Thread thread = new Thread(() =>
            {
                DirectoryInfo rootDir = new DirectoryInfo(_options.RootDirectoryPath);

                foreach (FileInfo fileInfo in rootDir.EnumerateFiles())
                {
                    CheckAndWaitForFileQueue();

                    _fileWorkingQueue.Enqueue(fileInfo.FullName);
                }

                foreach (DirectoryInfo dir in rootDir.EnumerateDirectories())
                {
                    _directoryStack.Push(dir.FullName);
                }

                string dirPath;
                while (_directoryStack.TryPop(out dirPath))
                {
                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo(dirPath);
                        foreach (FileInfo fileInfo in dir.EnumerateFiles())
                        {
                            CheckAndWaitForFileQueue();
                            _fileWorkingQueue.Enqueue(fileInfo.FullName);
                        }

                        foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
                        {
                            _directoryStack.Push(subDir.FullName);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                _directoryEnumerationFinished = true;
            });

            thread.Start();
        }

        private static void CheckAndWaitForFileQueue()
        {
            if (_fileQueueLimit < 1) return;

            while (_fileWorkingQueue.Count >= _fileQueueLimit)
            {
                Thread.Sleep(1000);
            }
        }

        private static void StartBackgroundEnumeration()
        {
            //TODO: multi thread this to make it run faster
            _backgroundEnumerationFinished = false;
            Status.TotalFileCount = 0;
            Status.TotalDirectoryCount = 0;

            Thread thread = new Thread(() =>
            {
                DirectoryInfo rootDir = new DirectoryInfo(_options.RootDirectoryPath);
                Interlocked.Add(ref Status.TotalFileCount, rootDir.EnumerateFiles().Count());
                Interlocked.Increment(ref Status.TotalDirectoryCount);

                foreach (DirectoryInfo subDir in rootDir.EnumerateDirectories())
                {
                    _backgroundDirectoryStack.Push(subDir.FullName);
                    Interlocked.Increment(ref Status.TotalDirectoryCount);
                }

                string dirPath;
                while (_backgroundDirectoryStack.TryPop(out dirPath))
                {
                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo(dirPath);
                        Interlocked.Add(ref Status.TotalFileCount, dir.EnumerateFiles().Count());
                        foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
                        {
                            _backgroundDirectoryStack.Push(subDir.FullName);
                            Interlocked.Increment(ref Status.TotalDirectoryCount);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                _backgroundEnumerationFinished = true;
                Status.EnumerationFinished = true;
            });

            thread.Start();
        }

        /// <summary>
        /// Performs the initial enumeration of the root directory,
        /// will return false if it fails for any reason.
        /// </summary>
        /// <returns></returns>
        private static bool TryInitialEnumeration()
        {
            DirectoryInfo dir = new DirectoryInfo(_options.RootDirectoryPath);

            try
            {
                //just attempt to enumerate subdir to make sure it works,
                //not bothering to check for exists first as this will still
                //achieve same end
                if (dir.EnumerateDirectories().Count() == 0 && dir.EnumerateFiles().Count() == 0)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static void BeginImpersonation()
        {
            if (_options.Credentials != null)
            {
                _impersonationContext?.Dispose();

                string domain = _options.Credentials.Domain ?? ".";
                string user = _options.Credentials.UserName;
                SecureString password = _options.Credentials.SecurePassword;

                _impersonationContext = Impersonation.LogonUser(domain, user, password, LogonType.Interactive);
            }
        }

        private static void EndImpersonation()
        {
            _impersonationContext?.Dispose();
        }

       
        /// <summary>
        /// Adds the option groups to the results property so that
        /// we can easily add results to it without having to worry
        /// about double adding matches
        /// </summary>
        private static void AddGroupsToResultProperty()
        {
            if (_options.HashGroups != null)
            {
                Results.HashGroups = _options.HashGroups;
            }
            else
            {
                Results.HashGroups = new HashInfoGroup[0];
            }

            if (_options.RegexGroups != null)
            {
                Results.RegexGroups = _options.RegexGroups;
            }
            else
            {
                Results.RegexGroups = new RegexInfoGroup[0];
            }
        }

        private static void AddNewScanEvent(ScanEventType type, DateTime stamp)
        {
            Status.ScanEvents.Add(new ScanEvent()
            {
                EventType = type,
                EventTimeStamp = stamp
            });
        }

        private static void AddRegexToInternalList()
        {
            if(_options.RegexGroups != null)
            {
                foreach (RegexInfoGroup group in _options.RegexGroups)
                {
                    if (group.RegexInfos != null && group.RegexInfos.Count > 0)
                    {
                        _includeRegex = true;
                        foreach (RegexInfo regexInfo in group.RegexInfos)
                        {
                            //This will trigger compiling the regex's which are then
                            //accessible as .AsRegex() to speed it all up later
                            if (regexInfo.IsValid)
                            {
                                _regexInfos.Add(regexInfo);
                            }
                           
                        }
                    }
                }
            }
        }

        private static void AddHashesToInternalSets()
        {
            if (_options.HashGroups != null)
            {
                foreach (HashInfoGroup hashGroup in _options.HashGroups)
                {
                    if (hashGroup.MD5s != null && hashGroup.MD5s.Count > 0)
                    {
                        _includeMD5 = true;
                        foreach (HashInfo thisInfo in hashGroup.MD5s)
                        {
                            _md5HashSet.Add(thisInfo.Hash);
                        }
                    }

                    if (hashGroup.SHA1s != null && hashGroup.SHA1s.Count > 0)
                    {
                        _includeSHA1 = true;
                        foreach (HashInfo thisInfo in hashGroup.SHA1s)
                        {
                            _sha1HashSet.Add(thisInfo.Hash);
                        }
                    }

                    if (hashGroup.SHA256s != null && hashGroup.SHA256s.Count > 0)
                    {
                        _includeSHA256 = true;
                        foreach (HashInfo thisInfo in hashGroup.SHA256s)
                        {
                            _sha256HashSet.Add(thisInfo.Hash);
                        }
                    }
                }
            }
        }
    }
}
