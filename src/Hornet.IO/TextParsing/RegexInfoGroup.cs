using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing
{
    public class RegexInfoGroup
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<RegexInfo> RegexInfos { get; set; }
        public IList<RegexResult> Matches { get; set; }
    }
}
