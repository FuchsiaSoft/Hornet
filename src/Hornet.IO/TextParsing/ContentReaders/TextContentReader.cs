using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing.ContentReaders
{
    internal class TextContentReader : IContentReader
    {
        private EncodingType _encodingType;

        public TextContentReader(EncodingType encodingType)
        {
            _encodingType = encodingType;
        }

        public bool TryGetContent(Stream fileStream, out string result)
        {
            result = string.Empty;
            if (fileStream.Length == 0) return false;

            try
            {
                using (StreamReader reader = GetStreamReader(fileStream))
                {
                    result = reader.ReadToEnd();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private StreamReader GetStreamReader(Stream fileStream)
        {
            switch (_encodingType)
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
    }
}
