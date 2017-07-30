using Hornet.IO.FileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Gets the current status of the scan as a <see cref="ScanStatus"/>
        /// </summary>
        public static ScanStatus Status { get; private set; } = new ScanStatus();

        public static ScanResult Results { get; private set; } = new ScanResult();

        /// <summary>
        /// Starts a scan running with the options provided.
        /// </summary>
        /// <param name="options">The options to run the scan with</param>
        public static void StartScan(ScanOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("Options must be specified starting a scan");
            }

            _options = options;

            AddNewScanEvent(ScanEventType.Start, DateTime.Now);

            AddHashesToInternalSets();


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
                }
            }
            if (options != null)
            {
                _includeMD5 = true;
                foreach (HashInfo info in options.MD5s)
                {
                    _md5HashSet.Add(info.Hash);
                }
            }

            if (options.SHA1s != null)
            {
                _includeSHA1 = true;
                foreach (HashInfo info in options.SHA1s)
                {
                    _sha1HashSet.Add(info.Hash);
                }
            }

            if (options.SHA256s != null)
            {
                _includeSHA256 = true;
                foreach (HashInfo info in options.SHA256s)
                {
                    _sha256HashSet.Add(info.Hash);
                }
            }
        }
    }
}
