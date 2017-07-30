using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing
{
    public class RegexResult
    {
        public string FilePath { get; set; }
        public string MimeType { get; set; }
        public string Length { get; set; }
        public IList<RegexInfo> MatchedRegexInfos { get; set; }
    }
}
