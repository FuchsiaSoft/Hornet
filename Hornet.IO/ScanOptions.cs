using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
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
        /// The list of <see cref="HashInfoGroup"/> objects to match against
        /// </summary>
        public IList<HashInfoGroup> HashGroups { get; set; } = new List<HashInfoGroup>();

        /// <summary>
        /// The list of <see cref="RegexInfoGroup"/> objects to match against
        /// </summary>
        public IList<RegexInfoGroup> RegexGroups { get; set; } = new List<RegexInfoGroup>();
    }
}
