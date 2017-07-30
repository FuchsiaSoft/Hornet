using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileInfo = Pri.LongPath.FileInfo;
using File = Pri.LongPath.File;
using Hornet.IO.TextParsing.ContentReaders;
using System.IO;

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

        public static string AttemptDecode(EncodingType type, string filePath)
        {
            string output;

            try
            {
                using (Stream fileStream = File.OpenRead(filePath))
                using (StreamReader reader = GetRightReader(type, fileStream))
                {
                    output = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                output = string.Empty;
            }

            return output;
        }

        private static StreamReader GetRightReader(EncodingType type, Stream fileStream)
        {
            switch (type)
            {
                case EncodingType.ASCII:
                    return new StreamReader(fileStream, Encoding.ASCII);

                case EncodingType.UTF7:
                    return new StreamReader(fileStream, Encoding.UTF7);

                case EncodingType.UTF8:
                    return new StreamReader(fileStream, Encoding.UTF8);

                case EncodingType.Unicode:
                    return new StreamReader(fileStream, Encoding.Unicode);

                case EncodingType.UnicodeBigEndian:
                    return new StreamReader(fileStream, Encoding.BigEndianUnicode);

                case EncodingType.UTF32:
                    return new StreamReader(fileStream, Encoding.UTF32);

                case EncodingType.AutoDetect:
                default:
                    return new StreamReader(fileStream, true);
            }
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
