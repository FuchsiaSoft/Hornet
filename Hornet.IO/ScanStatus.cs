using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO
{
    /**
     * ==============================================================================
     * Note on usage of Fields...
     * ==============================================================================
     * This class is intended to be a bit of a convenience so all of the properties
     * that could be considered part of a scan "status" are accessible in the same
     * place.  Some of the properties within it therefore must be fields rather than
     * proper properties with getters and setters, in order to be able to use
     * System.Threading.Interlocked from the scan manager to make the counters
     * go up.
     * ==============================================================================
     */


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
        /// Gets a <see cref="bool"/> that will return true
        /// when the scan is finished for any non-user issue
        /// (e.g. finished all files, failed to start etc.)
        /// </summary>
        public bool ScanFinished { get; internal set; } = false;

        /// <summary>
        /// Gets the current message for the scan
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// Gets a <see cref="long"/> value representing the total
        /// number of files (as presented on disk, not including embedded files) 
        /// that have been scanned so far for matching hashes
        /// </summary>
        public long TotalFilesHashScanned = 0;

        /// <summary>
        /// Gets a <see cref="long"/> value representing the total
        /// number of hashes that have been performed on files (as presented on disk, 
        /// not including embedded files) , including
        /// where the same file has been hashed multiple times with
        /// differing algorithms
        /// </summary>
        public long FileHashesPerformed = 0;

        /// <summary>
        /// Gets a <see cref="long"/> value representing the total number of
        /// hashes that have been performed on files embedded within other
        /// files, including where the same embedded file has been hashed
        /// multiple times with differing algorithms
        /// </summary>
        public long EmbeddedFileHashesPerformed = 0;

        /// <summary>
        /// Shortcut to returning <see cref="FileHashesPerformed"/> + <see cref="EmbeddedFileHashesPerformed"/>:
        /// Gets a <see cref="long"/> value representing the total number of
        /// hashes that have been performed on files and embedded files combined,
        /// including where the same file or embedded file has been hashes multiple
        /// times with differing algorithms
        /// </summary>
        public long TotalHashesPerformed { get { return FileHashesPerformed + EmbeddedFileHashesPerformed; } }

        /// <summary>
        /// Gets or sets a <see cref="int"/> representing the total number of
        /// directories that have been queued so far.
        /// </summary>
        public int TotalDirectoryCount  = 0;

        

        /// <summary>
        /// Gets or sets a <see cref="int"/> representing the total number
        /// of files that have been found during background enumeration.
        /// Remains zero if background enumeration is not enabled
        /// </summary>
        public int TotalFileCount = 0;

        public long TotalFilesSkipped = 0;
        public long TotalFilesSucceeded = 0;
        public long TotalFilesEncrypted = 0;
        public long TotalFilesFailed = 0;

        /// <summary>
        /// Gets the total number of bytes that have been processed (ignoring duplicate reads)
        /// </summary>
        public static long TotalBytesProcessed { get; internal set; } = 0;

        /// <summary>
        /// Gets a <see cref="IList{T}"/> with <see cref="T"/> as <see cref="ScanEvent"/>
        /// representing the significant events for the scan (starting, pausing, cancelling etc.)
        /// </summary>
        public IList<ScanEvent> ScanEvents { get; internal set; } = new List<ScanEvent>();
    }
}
