using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO
{
    /// <summary>
    /// Container object providing status information of the current scan
    /// </summary>
    public sealed class ScanStatus
    {
        /// <summary>
        /// Constructor is internal to prevent instantiation outside of
        /// <see cref="Hornet.IO"/> project
        /// </summary>
        internal ScanStatus() { }

        /// <summary>
        /// Gets a <see cref="bool"/> value indicating whether
        /// a scan is currently running.
        /// </summary>
        public bool ScanRunning { get; internal set; } = false;

        /// <summary>
        /// Gets a <see cref="long"/> value representing the total
        /// number of files (as presented on disk, not including embedded files) 
        /// that have been scanned so far for matching hashes
        /// </summary>
        public long TotalFilesHashScanned { get; internal set; } = 0;

        /// <summary>
        /// Gets a <see cref="long"/> value representing the total
        /// number of hashes that have been performed on files (as presented on disk, 
        /// not including embedded files) , including
        /// where the same file has been hashed multiple times with
        /// differing algorithms
        /// </summary>
        public long FileHashesPerformed { get; internal set; } = 0;

        /// <summary>
        /// Gets a <see cref="long"/> value representing the total number of
        /// hashes that have been performed on files embedded within other
        /// files, including where the same embedded file has been hashed
        /// multiple times with differing algorithms
        /// </summary>
        public long EmbeddedFileHashesPerformed { get; internal set; } = 0;

        /// <summary>
        /// Shortcut to returning <see cref="FileHashesPerformed"/> + <see cref="EmbeddedFileHashesPerformed"/>:
        /// Gets a <see cref="long"/> value representing the total number of
        /// hashes that have been performed on files and embedded files combined,
        /// including where the same file or embedded file has been hashes multiple
        /// times with differing algorithms
        /// </summary>
        public long TotalHashesPerformed { get { return FileHashesPerformed + EmbeddedFileHashesPerformed; } }

        /// <summary>
        /// Gets a <see cref="IList{T}"/> with <see cref="T"/> as <see cref="ScanEvent"/>
        /// representing the significant events for the scan (starting, pausing, cancelling etc.)
        /// </summary>
        public IList<ScanEvent> ScanEvents { get; internal set; } = new List<ScanEvent>();
    }
}
