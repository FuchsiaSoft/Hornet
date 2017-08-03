using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO
{
    public class ScanResult
    {
        private ConcurrentBag<string> _encryptedFiles = new ConcurrentBag<string>();

        public ScanStatus ClosingStatus { get; internal set; }
        public IEnumerable<HashInfoGroup> HashGroups { get; internal set; } = new HashInfoGroup[0];
        public IEnumerable<RegexInfoGroup> RegexGroups { get; internal set; } = new RegexInfoGroup[0];
        public IEnumerable<string> EncryptedFiles
        {
            get
            {
                return _encryptedFiles.ToArray();
            }
        }

        internal void AddEncryptedFile(string filePath)
        {
            _encryptedFiles.Add(filePath);
        }
    }
}
