using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using File = Pri.LongPath.File;
using FileInfo = Pri.LongPath.FileInfo;

namespace Hornet.IO.FileManagement
{
    internal class HashReader
    {
        private byte[] _fileBuffer = null;
        private bool _holdBuffer = false;
        private string _filePath = string.Empty;
        private FileInfo _fileInfo;
        private long _sizeOverride;

        public HashReader(string filePath, bool holdBufferInMemory, long sizeOverride = 0)
        {
            _holdBuffer = holdBufferInMemory;
            _filePath = filePath;
            _fileInfo = new FileInfo(filePath);
            _sizeOverride = sizeOverride;
        }

        /// <summary>
        /// Static helper for when a stream is already posessed (e.g. in memory
        /// files from an archive).  Does not utilise any internal buffers
        /// </summary>
        /// <param name="type">The <see cref="HashType"/> to use</param>
        /// <param name="stream">The stream to hash</param>
        /// <returns></returns>
        public static string GetHash(HashType type, Stream stream)
        {
            string output;

            try
            {
                switch (type)
                {
                    case HashType.MD5:
                        using (MD5 md5 = MD5.Create())
                        {
                            output = HashToString(md5.ComputeHash(stream));
                        }
                        break;

                    case HashType.SHA1:
                        using (SHA1 sha1 = SHA1.Create())
                        {
                            output = HashToString(sha1.ComputeHash(stream));
                        }
                        break;

                    case HashType.SHA256:
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            output = HashToString(sha256.ComputeHash(stream));
                        }
                        break;

                    default:
                        output = string.Empty;
                        break;
                }
            }
            catch (Exception)
            {
                output = string.Empty;
            }

            return output;
        }

        public string GetHash(HashType type)
        {
            string hashString;

            try
            {
                if (_holdBuffer == false)
                {
                    hashString = GetHashFromFile(type);
                }
                else
                {
                    if (_fileBuffer == null)
                    {
                        if (_sizeOverride == 0 || _fileInfo.Length < _sizeOverride)
                        {
                            ReadFileIntoMemory();
                            hashString = GetHashFromBuffer(type);
                        }
                        else
                        {
                            hashString = GetHashFromFile(type);
                        }
                    }
                    else
                    {
                        hashString = GetHashFromFile(type);
                    }
                }
            }
            catch (Exception)
            {
                hashString = string.Empty;
            }

            return hashString;
        }

        private void ReadFileIntoMemory()
        {
            _fileBuffer = File.ReadAllBytes(_filePath);
        }

        private string GetHashFromFile(HashType type)
        {
            string output;

            using (Stream fileStream = File.OpenRead(_filePath))
            {
                switch (type)
                {
                    case HashType.MD5:
                        using (MD5 md5 = MD5.Create())
                        {
                            output = HashToString(md5.ComputeHash(fileStream));
                        }
                        break;

                    case HashType.SHA1:
                        using (SHA1 sha1 = SHA1.Create())
                        {
                            output = HashToString(sha1.ComputeHash(fileStream));
                        }
                        break;

                    case HashType.SHA256:
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            output = HashToString(sha256.ComputeHash(fileStream));
                        }
                        break;

                    default:
                        output = string.Empty;
                        break;
                }
            }

            return output;
        }

        private string GetHashFromBuffer(HashType type)
        {
            string output;

            switch (type)
            {
                case HashType.MD5:
                    using (MD5 md5 = MD5.Create())
                    {
                        output = HashToString(md5.ComputeHash(_fileBuffer));
                    }
                    break;

                case HashType.SHA1:
                    using (SHA1 sha1 = SHA1.Create())
                    {
                        output = HashToString(sha1.ComputeHash(_fileBuffer));
                    }
                    break;

                case HashType.SHA256:
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        output = HashToString(sha256.ComputeHash(_fileBuffer));
                    }
                    break;

                default:
                    output = string.Empty;
                    break;
            }

            return output;
        }

        private static string HashToString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var hashByte in hash)
            {
                sb.Append(hashByte.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
