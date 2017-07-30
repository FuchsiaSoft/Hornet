using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.FileManagement
{
    public class HashResult
    {
        /// <summary>
        /// The path of the matched file
        /// </summary>
        public string FilePath { get; internal set; }

        /// <summary>
        /// The MIME type of the matched file
        /// </summary>
        public string MimeType { get; internal set; }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether
        /// the match was an embedded file
        /// </summary>
        public bool EmbeddedFile { get; internal set; } = false;

        /// <summary>
        /// The path of the parent file, if the file is embedded
        /// </summary>
        public string ParentPath { get; internal set; } = string.Empty;

        /// <summary>
        /// The length of the matched file
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// The <see cref="HashInfo"/> that was matched
        /// </summary>
        public HashInfo MatchedHashInfo { get; set; }
    }
}
