using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.FileManagement
{
    public class HashResult
    {
        public string FilePath { get; set; }
        public string MimeType { get; set; }
        public long Length { get; set; }
        public HashInfo MatchedHashInfo { get; set; }
    }
}
