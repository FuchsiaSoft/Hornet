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
    }
}
