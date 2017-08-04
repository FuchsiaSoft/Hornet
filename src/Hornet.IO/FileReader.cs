using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using Hornet.IO.TextParsing.ContentReaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Pri.LongPath.File;
using FileInfo = Pri.LongPath.FileInfo;
using Path = Pri.LongPath.Path;

namespace Hornet.IO
{
    public class FileReader
    {
        private string _filePath;
        private ScanOptions _options;
        private bool _includeMD5;
        private bool _includeRegex;
        private bool _includeSHA1;
        private bool _includeSHA256;

        public FileReader(string filePath, ScanOptions options, bool includeMD5, bool includeSHA1, bool includeSHA256, bool includeRegex)
        {
            _filePath = filePath;
            _options = options;
            _includeMD5 = includeMD5;
            _includeSHA1 = includeSHA1;
            _includeSHA256 = includeSHA256;
            _includeRegex = includeRegex;
        }

        public FileResult GetResult()
        {
            FileInfo fileInfo;
            if (!CanRead(out fileInfo))
            {
                return new FileResult() { ResultType = ResultType.Failed };
            }

            if (!ShouldParse(fileInfo))
            {
                return new FileResult() { ResultType = ResultType.Skipped };
            }

            try
            {
                using (Stream stream = GetStream(fileInfo))
                {
                    FileResult result = new FileResult();

                    DoHashReading(stream, result);

                    DoContentReading(stream, result);

                    return result;
                }
            }
            catch (Exception)
            {
                return new FileResult() { ResultType = ResultType.Failed };
            }
        }

        private void DoContentReading(Stream stream, FileResult result)
        {
            if (!_includeRegex) return;

            IContentReader reader = null;

            switch (Path.GetExtension(_filePath).ToUpper())
            {
                case ".TXT":
                    //TODO: pick up here
                    break;

                default:
                    if (_options.AttemptTextDecode) reader = new TextContentReader(_options.EncodingType);
                    break;
            }

            if (reader != null)
            {
                string output;
                if (reader.TryGetContent(stream, out output))
                {
                    result.Content = output;
                }
                else
                {
                    result.Content = string.Empty;
                }
            }
        }

        private void DoHashReading(Stream stream, FileResult result)
        {
            if (_includeMD5) result.MD5 = HashReader.GetMD5(stream, true);
            if (_includeSHA1) result.SHA1 = HashReader.GetSHA1(stream, true);
            if (_includeSHA256) result.SHA256 = HashReader.GetSHA256(stream, true);
        }

        private bool CanRead(out FileInfo fileInfo)
        {
            fileInfo = null;
            try
            {
                fileInfo = new FileInfo(_filePath);
                if (fileInfo.Exists)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Stream GetStream(FileInfo fileInfo)
        {
            if (_options.HoldBufferInMemory)
            {
                long sizeLimit = _options.InMemoryFileSizeLimit < 1 ? 0 : _options.InMemoryFileSizeLimit;
                if (sizeLimit == 0 || fileInfo.Length <= sizeLimit)
                {
                    MemoryStream memStream = new MemoryStream();
                    using (FileStream fileStream = fileInfo.OpenRead())
                    {
                        fileStream.CopyTo(memStream);
                        memStream.Seek(0, SeekOrigin.Begin);
                    }
                    return memStream;
                }
            }

            return fileInfo.OpenRead();
        }

        private bool ShouldParse(FileInfo fileInfo)
        {
            //TODO: full range of options here, need to check file types
            //amongst other things like size etc.
            if (_includeMD5 == false &&
                _includeSHA1 == false && 
                _includeSHA256 == false &&
                _includeRegex == false)
            {
                return false;
            }

            return true;
        }
    }
}
