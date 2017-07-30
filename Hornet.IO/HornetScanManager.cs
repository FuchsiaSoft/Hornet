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

        //Private flags for convenience instead of constantly checking the
        //hashset counts
        private static bool _includeMD5 = false;
        private static bool _includeSHA1 = false;
        private static bool _includeSHA256 = false;

        private static ScanOptions _options;

        //Some of the options are copied to here on start scan just
        //for the sake of concise syntax
        private static int _dirQueueLimit;
        private static int _fileQueueLimit;

        //Working queues used to avoid enormous amounts of recursion on SAN
        //scale file systems
        private static ConcurrentQueue<string> _directoryEnumerationQueue = new ConcurrentQueue<string>();
        private static ConcurrentQueue<string> _fileWorkingQueue = new ConcurrentQueue<string>();

        private static Thread _directoryEnumerationThread;
        private static Thread _fileEnumerationThread;
        private static List<Thread> _workerThreads = new List<Thread>();

        //Flags for maintaining state with all the different threads.
        private static bool _directoryEnumerationFinished = false;
        private static bool _fileEnumerationFinished = false;


        //Used when credentials are provided so the scan manager will operate within
        //this impersonation context
        private static Impersonation _impersonationContext;

        //Hashset for checking whether to bother attempting the content
        //of a file based on its extension
        private static HashSet<string> _validExtensions = new HashSet<string>();

        //various sets of file extensions
        private static List<string> _pdfs = new List<string>()
        {
            ".pdf",
            ".PDF"
        };

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
            _dirQueueLimit = _options.MaxDirectoryQueueSize;
            _fileQueueLimit = _options.MaxFileQueueSize;

            AddNewScanEvent(ScanEventType.Start, DateTime.Now);

            BeginImpersonation();

            Status.ScanRunning = true;
            Status.Message = "Preparing internal data...";

            await Task.Run(() =>
            {
                AddHashesToInternalSets();

                AddGroupsToResultProperty();

                Status.Message = "Commencing worker threads...";

                if (!TryInitialEnumeration())
                {
                    Status.ScanRunning = false;
                    Status.ScanFinished = true;
                    Status.Message = "Failed initial enumeration";
                    AddNewScanEvent(ScanEventType.Finish, DateTime.Now);
                }

                EstablishExtensionsForContent();

                StartDirectoryEnumerationThread();

                StartFileEnumerationThread();

                DoAssignment();
            });
        }

        private static void EstablishExtensionsForContent()
        {
            //TODO: check options properly
            foreach (string extension in _pdfs)
            {
                _validExtensions.Add(extension);
            }
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

        private static void DoAssignment()
        {
            int threadCount = _options.MaxWorkerThreads < 1 ? 1 : _options.MaxWorkerThreads;

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(DoWork);
                _workerThreads.Add(thread);
                thread.Start();
            }
        }

        private static void DoWork()
        {
            string filePath;

            bool holdBuffer = false;
            if (_options.HoldBufferForMultipleHashes)
            {
                int hashAlgoCount = 0;

                if (_includeMD5) hashAlgoCount++;
                if (_includeSHA1) hashAlgoCount++;
                if (_includeSHA256) hashAlgoCount++;

                if (hashAlgoCount > 1) holdBuffer = true;
            }

            long sizeOverride = _options.MaxBufferSize < 0 ? 0 : _options.MaxBufferSize;

            while (_fileEnumerationFinished == false || _fileWorkingQueue.Count > 0)
            {
                if (_fileWorkingQueue.Count == 0)
                {
                    Thread.Sleep(1000);
                }

                while (_fileWorkingQueue.TryDequeue(out filePath))
                {
                    HashReader hashReader = new HashReader(filePath, holdBuffer, sizeOverride);

                    ProcessFileForHashes(filePath, hashReader);

                    //TODO: embedded files here
                    if (_options.IncludeZip && ZipFile.IsZipFile(filePath))
                    {
                        ProcessZip(filePath);
                    }

                    FileReader contentReader = new FileReader(filePath);

                    
                }
            }
        }

        private static void ProcessZip(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);

                using (ZipFile zip = ZipFile.Read(filePath))
                {
                    bool? useDisk = CheckWhetherToUseDisk(fileInfo);

                    if (useDisk == null) return; //skip the zip file, not allowed to use disk but it's too big

                    foreach (ZipEntry entry in zip.Entries)
                    {
                        if ((bool)useDisk)
                        {
                            string tempFile = Path.GetTempFileName();
                            using (Stream stream = File.OpenWrite(tempFile))
                            {
                                ProcessEmbeddedFileStream(filePath, entry, stream);
                            }
                            File.Delete(tempFile);
                        }
                        else
                        {
                            using (Stream stream = new MemoryStream())
                            {
                                ProcessEmbeddedFileStream(filePath, entry, stream);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private static bool? CheckWhetherToUseDisk(FileInfo fileInfo)
        {
            if (_options.UnzipInMemory)
            {
                long zipSizeOverride = _options.MaxZipInMemorySize < 0 ? 0 : _options.MaxZipInMemorySize;
                if (zipSizeOverride == 0 || fileInfo.Length < zipSizeOverride)
                {
                    return false;
                }
                else
                {
                    if (_options.UnzipToDiskIfTooBig)
                    {
                        return true;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        private static void ProcessEmbeddedFileStream(string filePath, ZipEntry entry, Stream stream)
        {
            entry.Extract(stream);

            if (_includeMD5)
            {
                string md5 = HashReader.GetHash(HashType.MD5, stream);
                if (_md5HashSet.Contains(md5))
                    AddResultToMatchedGroups(md5, HashType.MD5, filePath, true, entry.FileName, entry.UncompressedSize);
            }

            if (_includeSHA1)
            {
                string sha1 = HashReader.GetHash(HashType.SHA1, stream);
                if (_md5HashSet.Contains(sha1))
                    AddResultToMatchedGroups(sha1, HashType.SHA1, filePath, true, entry.FileName, entry.UncompressedSize);
            }

            if (_includeSHA256)
            {
                string sha256 = HashReader.GetHash(HashType.SHA256, stream);
                if (_md5HashSet.Contains(sha256))
                    AddResultToMatchedGroups(sha256, HashType.SHA256, filePath, true, entry.FileName, entry.UncompressedSize);
            }
        }

        private static void ProcessFileForHashes(string filePath, HashReader hashReader)
        {
            if (_includeMD5)
            {
                string md5 = hashReader.GetHash(HashType.MD5);
                if (_md5HashSet.Contains(md5)) AddResultToMatchedGroups(md5, HashType.MD5, filePath);
            }

            if (_includeSHA1)
            {
                string sha1 = hashReader.GetHash(HashType.SHA1);
                if (_sha1HashSet.Contains(sha1)) AddResultToMatchedGroups(sha1, HashType.SHA1, filePath);
            }

            if (_includeSHA256)
            {
                string sha256 = hashReader.GetHash(HashType.SHA256);
                if (_sha256HashSet.Contains(sha256)) AddResultToMatchedGroups(sha256, HashType.SHA256, filePath);
            }
        }

        private static void AddResultToMatchedGroups(string hash, HashType type, string filePath, bool embedded = false, string embeddedName = "", long embeddedSize = 0)
        {
            try
            {
                foreach (HashInfoGroup hashGroup in Results.HashGroups)
                {
                    List<HashInfo> matchedHashInfos = new List<HashInfo>();
                    if (type == HashType.MD5) matchedHashInfos.AddRange(hashGroup.MD5s.Where(h => h.Hash == hash));
                    if (type == HashType.SHA1) matchedHashInfos.AddRange(hashGroup.SHA1s.Where(h => h.Hash == hash));
                    if (type == HashType.SHA256) matchedHashInfos.AddRange(hashGroup.SHA256s.Where(h => h.Hash == hash));

                    foreach (HashInfo hashInfo in matchedHashInfos)
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        string mimeType;
                        if (embedded)
                        {
                            mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(embeddedName ?? string.Empty));
                        }
                        else
                        {
                            mimeType = MimeTypeMap.GetMimeType(fileInfo.Extension ?? string.Empty);
                        }

                        HashResult result = new HashResult()
                        {
                            EmbeddedFile = embedded,
                            ParentPath = embedded ? filePath : string.Empty,
                            FilePath = embedded ? embeddedName : filePath,
                            ParentLength = embedded ? fileInfo.Length : 0,
                            Length = embedded ? embeddedSize : fileInfo.Length,
                            MatchedHashInfo = hashInfo,
                            MimeType = mimeType
                        };

                        hashGroup.Matches.Add(result);
                    }
                }
            }
            catch (Exception)
            {
                //TODO: implement proper logging here
                return;
            }
        }

        private static void StartDirectoryEnumerationThread()
        {
            _directoryEnumerationThread = new Thread(DoDirectoryEnumeration);
            _directoryEnumerationThread.Start();
        }

        private static void DoDirectoryEnumeration()
        {
            DirectoryInfo rootDir = new DirectoryInfo(_options.RootDirectoryPath);

            RecurseDirectory(rootDir);

            lock (Status)
            {
                Status.FinalDirectoryCount = Status.TotalDirectoryCount;
            }
        }

        private static void RecurseDirectory(DirectoryInfo dir)
        {
            try
            {
                foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
                {
                    RecurseDirectory(subDir);
                    while (_dirQueueLimit > 0 && _directoryEnumerationQueue.Count > _dirQueueLimit)
                    {
                        Thread.Sleep(1000);
                    }

                    _directoryEnumerationQueue.Enqueue(subDir.FullName);
                    Interlocked.Increment(ref Status.TotalDirectoryCount);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private static void StartFileEnumerationThread()
        {
            _fileEnumerationThread = new Thread(DoFileEnumeration);
            _fileEnumerationThread.Start();
        }

        private static void DoFileEnumeration()
        {
            string dirPath;

            while (!_directoryEnumerationFinished)
            {
                if (_directoryEnumerationQueue.Count == 0)
                {
                    //Right at the beginning there won't be anything in this
                    //queue for a while so chillax
                    Thread.Sleep(5000);
                }

                while (_directoryEnumerationQueue.TryPeek(out dirPath))
                {
                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo(dirPath);
                        IEnumerable<FileInfo> files = dir.EnumerateFiles();
                        foreach (FileInfo file in files)
                        {
                            //if we have directories in queue but no room for them, just chillax here too
                            while (_fileQueueLimit > 0 && _fileWorkingQueue.Count >= _fileQueueLimit)
                            {
                                Thread.Sleep(5000);
                            }

                            _fileWorkingQueue.Enqueue(file.FullName);
                            Interlocked.Increment(ref Status.TotalFileCount);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            _fileEnumerationFinished = true;
        }

        private static void AddGroupsToResultProperty()
        {
            Results.HashGroups = _options.HashGroups ?? new HashInfoGroup[0];
            Results.RegexGroups = _options.RegexGroups ?? new RegexInfoGroup[0];
        }

        private static void AddNewScanEvent(ScanEventType type, DateTime stamp)
        {
            Status.ScanEvents.Add(new ScanEvent()
            {
                EventType = type,
                EventTimeStamp = stamp
            });
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
