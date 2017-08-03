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
    public class HashReader
    {
        //TODO: review all accessibility modifiers!

        
        public static string GetMD5(Stream stream, bool resetStream = true)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(stream);
                string output = HashToString(hash);
                if (resetStream) stream.Seek(0, SeekOrigin.Begin);
                return output;
            }
        }

        public static string GetSHA1(Stream stream, bool resetStream = true)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(stream);
                string output = HashToString(hash);
                if (resetStream) stream.Seek(0, SeekOrigin.Begin);
                return output;
            }
        }

        public static string GetSHA256(Stream stream, bool resetStream = true)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(stream);
                string output = HashToString(hash);
                if (resetStream) stream.Seek(0, SeekOrigin.Begin);
                return output;
            }
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
