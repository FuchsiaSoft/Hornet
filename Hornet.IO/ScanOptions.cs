using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO
{
    /// <summary>
    /// Object for containing all scan options to be provided
    /// to the <see cref="HornetScanManager"/> on
    /// <see cref="HornetScanManager.StartScan(ScanOptions)"/>
    /// </summary>
    public class ScanOptions
    {
        /// <summary>
        /// The credentials to use for impersonation if required
        /// </summary>
        public NetworkCredential Credentials { get; set; }

        /// <summary>
        /// The path to serve as root directory for the scan
        /// </summary>
        public string RootDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the maximum directory queue size, zero or
        /// any non positive int is counted as unlimited
        /// </summary>
        public int MaxDirectoryQueueSize { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum number of worker threads to use,
        /// any number less than 1 is treated as 1
        /// </summary>
        public int MaxWorkerThreads { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum file queue size, zero or any
        /// non positive int is counted as unlimited
        /// </summary>
        public int MaxFileQueueSize { get; set; } = 0;

        /// <summary>
        /// The timeout in seconds to apply to regex's
        /// </summary>
        public int RegexTimeoutSeconds { get; set; } = 0;

        /// <summary>
        /// Gets or sets a <see cref="bool"/> indicating whether or not
        /// to hold the buffer in memory for files when running multiple
        /// hashes
        /// </summary>
        public bool HoldBufferForMultipleHashes { get; set; } = false;

        /// <summary>
        /// Gets or sets a <see cref="long"/> representing the maximum length
        /// of a file to hold in an in-memory buffer if <see cref="HoldBufferForMultipleHashes"/>
        /// is set to true.  Zero is treated as unlimited.
        /// </summary>
        public long MaxBufferSize { get; set; } = 0;

        /// <summary>
        /// The list of <see cref="HashInfoGroup"/> objects to match against
        /// </summary>
        public IList<HashInfoGroup> HashGroups { get; set; } = new List<HashInfoGroup>();

        /// <summary>
        /// The list of <see cref="RegexInfoGroup"/> objects to match against
        /// </summary>
        public IList<RegexInfoGroup> RegexGroups { get; set; } = new List<RegexInfoGroup>();
    }
}
