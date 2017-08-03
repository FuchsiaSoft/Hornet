using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileInfo = Pri.LongPath.FileInfo;

namespace Hornet.IO
{
    internal class FileResult
    {
        public ResultType ResultType { get; set; }
        public string Name { get; set; }
        public long Length { get; set; }
        public string MD5 { get; set; } = null;
        public string SHA1 { get; set; } = null;
        public string SHA256 { get; set; } = null;
        public string Content { get; set; } = null;
        public List<FileResult> EmbeddedResults { get; set; } = new List<FileResult>();
    }
}
