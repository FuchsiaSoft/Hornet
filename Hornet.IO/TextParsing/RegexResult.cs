using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = Pri.LongPath.Path;

namespace Hornet.IO.TextParsing
{
    public class RegexResult
    {
        /// <summary>
        /// The name of the matched file
        /// </summary>
        public string Name { get; set; }

        public string ShortName
        {
            get
            {
                return Path.GetFileName(Name);
            }
        }

        /// <summary>
        /// The MIME type of the matched file
        /// </summary>
        public string MimeType { get; internal set; }

        /// <summary>
        /// The length of the matched file
        /// </summary>
        public long Length { get; internal set; }

        /// <summary>
        /// The <see cref="RegexInfo"/> objects that were matched
        /// </summary>
        public IEnumerable<Tuple<RegexInfo, string>> MatchedRegexInfos { get; internal set; }
    }
}
