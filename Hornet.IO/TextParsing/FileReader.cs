using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pri.LongPath;
using FileInfo = Pri.LongPath.FileInfo;
using Hornet.IO.TextParsing.ContentReaders;

namespace Hornet.IO.TextParsing
{
    public class FileReader
    {
        private string _fileBody = string.Empty;
        private bool _alreadyChecked = false;
        private FileInfo _fileInfo;

        private Dictionary<string, Type> _readerLookup = new Dictionary<string, Type>()
        {
            {".DOC", typeof(BinaryWordDocContentReader) }
        };

        public bool HasContent
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_fileBody))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public string Content
        {
            get
            {
                if (!_alreadyChecked)
                {
                    TryParse();
                }
                return _fileBody;
            }
        }

        public FileReader(string path)
        {
            _fileInfo = new FileInfo(path);
        }
        
        public bool TryParse()
        {
            if (_fileInfo.Exists == false)
            {
                _alreadyChecked = true;
                return false;
            }

            IContentReader reader = ResolveReader(_fileInfo);
            if (reader != null)
            {
                string result;
                return reader.TryGetContent(_fileInfo.FullName, out result);
            }

            return false;
        }

        private IContentReader ResolveReader(FileInfo _fileInfo)
        {
            string extension = _fileInfo.Extension.ToUpper();

            if (_readerLookup.ContainsKey(extension))
            {
                return (IContentReader)Activator.CreateInstance(_readerLookup[extension]);
            }

            return null;
        }
    }
}
