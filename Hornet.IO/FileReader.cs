using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO
{
    internal class FileReader
    {
        private bool _includeMD5;
        private bool _includeRegex;
        private bool _includeSHA1;
        private bool _includeSHA256;

        public FileReader(string filePath, ScanOptions options, bool includeMD5, bool includeSHA1, bool includeSHA256, bool includeRegex)
        {
            _includeMD5 = includeMD5;
            _includeSHA1 = includeSHA1;
            _includeSHA256 = includeSHA256;
            _includeRegex = includeRegex;
        }

        public FileResult GetResult()
        {
            throw new NotImplementedException();
        }
    }
}
