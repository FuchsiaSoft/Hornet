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
using SimpleImpersonation;
using System.Security;
using Pri.LongPath;

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

        //Working queues used to avoid enormous amounts of recursion on SAN
        //scale file systems
        private static ConcurrentQueue<string> _directoryEnumerationQueue = new ConcurrentQueue<string>();
        private static ConcurrentQueue<string> _directoryWorkingQueue = new ConcurrentQueue<string>();
        private static ConcurrentQueue<string> _fileWorkingQueue = new ConcurrentQueue<string>();

        private static Thread _directoryEnumerationThread;
        private static Thread _fileEnumerationThread;
        private static Thread _assignerThread;

        //Flags for maintaining state with all the different threads.
        private static bool _directoryEnumerationFinished = false;
        private static bool _fileEnumerationFinished = false;


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

                StartDirectoryEnumerationThread();

                StartFileEnumerationThread();

                StartAssignerThread();
            });
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

        private static void StartAssignerThread()
        {
            throw new NotImplementedException();
        }

        private static void StartDirectoryEnumerationThread()
        {
            _directoryEnumerationThread = new Thread(DoDirectoryEnumeration);
            _directoryEnumerationThread.Start();
        }

        private static void DoDirectoryEnumeration()
        {
            DirectoryInfo rootDir = new DirectoryInfo(_options.RootDirectoryPath);



            string dirPath;
            int queueLimit = _options.MaxDirectoryQueueSize;

            while (_directoryEnumerationQueue.TryDequeue(out dirPath))
            {
                DirectoryInfo dir = new DirectoryInfo(dirPath);

                RecurseDirectory(dir);

                try
                {
                    IEnumerable<DirectoryInfo> subDirs = dir.EnumerateDirectories();

                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        while (queueLimit > 0 && _directoryWorkingQueue.Count >= queueLimit)
                        {
                            Thread.Sleep(1000);
                        }

                        _directoryEnumerationQueue.Enqueue(subDir.FullName);
                        _directoryWorkingQueue.Enqueue(subDir.FullName);

                        Interlocked.Increment(ref Status.TotalDirectoryCount);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            _directoryEnumerationFinished = true;

            lock (Status)
            {
                Status.FinalDirectoryCount = Status.TotalDirectoryCount;
            }
        }

        private static void RecurseDirectory(DirectoryInfo dir)
        {
            throw new NotImplementedException();
        }

        private static void StartFileEnumerationThread()
        {
            _fileEnumerationThread = new Thread(DoFileEnumeration);
            _fileEnumerationThread.Start();
        }

        private static void DoFileEnumeration()
        {
            string dirPath;
            int queueLimit = _options.MaxFileQueueSize;

            while (_directoryWorkingQueue.TryDequeue(out dirPath))
            {
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                try
                {
                    IEnumerable<FileInfo> files = dir.EnumerateFiles();

                    foreach (FileInfo file in files)
                    {
                        while (queueLimit > 0 && _fileWorkingQueue.Count >= queueLimit)
                        {
                            Thread.Sleep(1000);
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
