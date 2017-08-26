using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.FileManagement
{
    /// <summary>
    /// Provides a container for all the hashes and their associated info
    /// for matching
    /// </summary>
    public class HashInfoGroup
    {
        /// <summary>
        /// The name of the hash info group for reporting
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The detailed description of the hash info group for reporting
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a list of MD5 hashes to scan for
        /// </summary>
        public List<HashInfo> MD5s { get; set; } = new List<HashInfo>();

        /// <summary>
        /// Gets or sets a list of SHA1 hashes to scan for
        /// </summary>
        public List<HashInfo> SHA1s { get; set; } = new List<HashInfo>();

        /// <summary>
        /// Gets or sets a list of SHA256 hashes to scan for
        /// </summary>
        public List<HashInfo> SHA256s { get; set; } = new List<HashInfo>();

        /// <summary>
        /// Gets the list of <see cref="HashResult"/> objects representing the matches
        /// that have been added for this group
        /// </summary>
        [JsonIgnore]
        public List<HashResult> Matches { get; internal set; } = new List<HashResult>();

        public void SaveToFile(string filePath)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (FileStream fileStream = File.Create(filePath))
            using (ZipFile zip = new ZipFile())
            {
                byte[] fileBytes = Encoding.UTF8.GetBytes(json);

                zip.AddEntry("Content", fileBytes);

                zip.Save(fileStream);
            }
        }

        public static HashInfoGroup FromFile(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                using (ZipFile zip = ZipFile.Read(filePath))
                using (MemoryStream stream = new MemoryStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    zip.Entries.First(e => e.FileName == "Content").Extract(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    string json = reader.ReadToEnd();
                    HashInfoGroup group = JsonConvert.DeserializeObject<HashInfoGroup>(json);
                    return group;
                }
            }
            catch (Exception)
            {
                throw new FileFormatException("Not a valid hashset file");
            }
        }
    }
}
