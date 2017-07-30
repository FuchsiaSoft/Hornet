using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO
{
    public class ScanResult
    {
        public IEnumerable<HashInfoGroup> HashGroups { get; internal set; } = new HashInfoGroup[0];
        public IEnumerable<RegexInfoGroup> RegexGroups { get; internal set; } = new RegexInfoGroup[0];
    }
}
