using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using Hornet.IO.TextParsing.ContentReaders;
using Ionic.Zip;
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

                    result.ResultType = ResultType.Read;
                    result.Length = fileInfo.Length;
                    result.Name = fileInfo.FullName;

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

            string extension = Path.GetExtension(_filePath.ToUpper());

            switch (extension)
            {
                case ".TXT":
                case ".CSV":
                case ".XML":
                    reader = new TextContentReader(EncodingType.AutoDetect);
                    break;

                case ".PDF":
                    reader = new PdfContentReader();
                    break;

                case ".HTML":
                    reader = new HtmlContentReader();
                    break;

                case ".DOCX":
                    reader = new DocxContentReader();
                    break;

                default:
                    if (_options.AttemptTextDecode)
                    {
                        if (!_options.ExcludedExtensionsForTextAttempt.Any(s=>s.ToUpper() == extension))
                        {
                            reader = new TextContentReader(_options.EncodingType);
                        }
                    }
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

            if (_options.HashIncludeZip)
            {
                List<FileResult> embeddedHashResults = new List<FileResult>();
                ReadZipForHashes(stream, embeddedHashResults);

                result.EmbeddedResults.AddRange(embeddedHashResults);
            }
        }

        //TODO: did this in a bit of a rush, needs a lot of re factoring to make it DRY
        private void ReadZipForHashes(Stream stream, List<FileResult> embeddedHashResults)
        {
            stream.Seek(0, SeekOrigin.Begin);
            if (ZipFile.IsZipFile(stream, true))
            {
                if (_options.UnzipInMemory)
                {
                    if (_options.MaxZipInMemorySize < 1 || _options.MaxZipInMemorySize > stream.Length)
                    {
                        //We use in-memory here
                        stream.Seek(0, SeekOrigin.Begin);
                        using (ZipFile zipFile = ZipFile.Read(stream))
                        {
                            foreach (ZipEntry entry in zipFile.Entries.Where(e => e.IsDirectory == false))
                            {
                                using (MemoryStream extractStream = new MemoryStream())
                                {
                                    entry.Extract(extractStream);
                                    extractStream.Seek(0, SeekOrigin.Begin);

                                    FileResult embeddedResult = new FileResult()
                                    {
                                        Name = entry.FileName,
                                        Length = entry.UncompressedSize
                                    };

                                    if (_includeMD5) embeddedResult.MD5 = HashReader.GetMD5(extractStream, true);
                                    if (_includeSHA1) embeddedResult.SHA1 = HashReader.GetSHA1(extractStream, true);
                                    if (_includeSHA256) embeddedResult.SHA256 = HashReader.GetSHA256(extractStream, true);

                                    embeddedHashResults.Add(embeddedResult);

                                    extractStream.Seek(0, SeekOrigin.Begin);
                                    if (ZipFile.IsZipFile(extractStream, true))
                                    {
                                        ReadZipForHashes(extractStream, embeddedHashResults);
                                    }
                                }
                            }
                        }
                    }
                    else if (_options.UnzipToDiskIfTooBig)
                    {
                        //use local disk
                        stream.Seek(0, SeekOrigin.Begin);
                        using (ZipFile zipFile = ZipFile.Read(stream))
                        {
                            foreach (ZipEntry entry in zipFile.Entries.Where(e => e.IsDirectory == false))
                            {
                                string tempFileName = Path.GetTempFileName();
                                
                                using (FileStream extractStream = File.OpenWrite(tempFileName))
                                {
                                    entry.Extract(extractStream);
                                    extractStream.Seek(0, SeekOrigin.Begin);

                                    FileResult embeddedResult = new FileResult()
                                    {
                                        Name = entry.FileName,
                                        Length = entry.UncompressedSize
                                    };

                                    if (_includeMD5) embeddedResult.MD5 = HashReader.GetMD5(extractStream, true);
                                    if (_includeSHA1) embeddedResult.SHA1 = HashReader.GetSHA1(extractStream, true);
                                    if (_includeSHA256) embeddedResult.SHA256 = HashReader.GetSHA256(extractStream, true);

                                    embeddedHashResults.Add(embeddedResult);

                                    extractStream.Seek(0, SeekOrigin.Begin);
                                    if (ZipFile.IsZipFile(extractStream, true))
                                    {
                                        ReadZipForHashes(extractStream, embeddedHashResults);
                                    }
                                }

                                File.Delete(tempFileName);
                            }
                        }
                    }
                    else
                    {
                        //we skip the zip here, nothing
                    }
                }
                else
                {
                    //will always use local disk here
                    stream.Seek(0, SeekOrigin.Begin);
                    using (ZipFile zipFile = ZipFile.Read(stream))
                    {
                        foreach (ZipEntry entry in zipFile.Entries.Where(e => e.IsDirectory == false))
                        {
                            string tempFileName = Path.GetTempFileName();

                            using (FileStream extractStream = File.OpenWrite(tempFileName))
                            {
                                entry.Extract(extractStream);
                                extractStream.Seek(0, SeekOrigin.Begin);

                                FileResult embeddedResult = new FileResult()
                                {
                                    Name = entry.FileName,
                                    Length = entry.UncompressedSize
                                };

                                if (_includeMD5) embeddedResult.MD5 = HashReader.GetMD5(extractStream, true);
                                if (_includeSHA1) embeddedResult.SHA1 = HashReader.GetSHA1(extractStream, true);
                                if (_includeSHA256) embeddedResult.SHA256 = HashReader.GetSHA256(extractStream, true);

                                embeddedHashResults.Add(embeddedResult);

                                extractStream.Seek(0, SeekOrigin.Begin);
                                if (ZipFile.IsZipFile(extractStream, true))
                                {
                                    ReadZipForHashes(extractStream, embeddedHashResults);
                                }
                            }

                            File.Delete(tempFileName);
                        }
                    }
                }
            }
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

            if (fileInfo.Length > _options.MaxSizeToAttemptHash &&
                _options.MaxSizeToAttemptHash > 1)
            {
                return false;
            }

            return true;
        }
    }
}
