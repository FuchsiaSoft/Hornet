using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = Pri.LongPath.Path;

namespace Hornet.IO.FileManagement
{
    public class HashResult
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
        /// The <see cref="HashInfo"/> that was matched
        /// </summary>
        public IEnumerable<HashInfo> MatchedHashInfos { get; internal set; }
    }
}
