using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing
{
    public class RegexInfoGroup
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RegexInfo> RegexInfos { get; set; } = new List<RegexInfo>();
        [JsonIgnore]
        public List<RegexResult> Matches { get; set; }

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

        public static RegexInfoGroup FromFile(string filePath)
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
                    RegexInfoGroup group = JsonConvert.DeserializeObject<RegexInfoGroup>(json);
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
