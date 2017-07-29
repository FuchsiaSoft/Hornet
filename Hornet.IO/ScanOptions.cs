using Hornet.IO.FileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Gets or sets a list of MD5 hashes to scan for
        /// </summary>
        public IList<HashInfo> MD5s { get; set; } = new List<HashInfo>();

        /// <summary>
        /// Gets or sets a list of SHA1 hashes to scan for
        /// </summary>
        public IList<HashInfo> SHA1s { get; set; } = new List<HashInfo>();

        /// <summary>
        /// Gets or sets a list of SHA256 hashes to scan for
        /// </summary>
        public IList<HashInfo> SHA256s { get; set; } = new List<HashInfo>();
    }
}
