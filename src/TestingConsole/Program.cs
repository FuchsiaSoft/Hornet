using Hornet.IO;
using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ScanOptions options = new ScanOptions()
            {
                RegexGroups = new List<RegexInfoGroup>()
                {
                    new RegexInfoGroup()
                    {
                        RegexInfos = new List<RegexInfo>()
                        {
                            new RegexInfo() { Pattern = "(?i)(trigger)" }
                        }
                    }
                }
            };
            FileReader reader = new FileReader(@"H:\Technical options CSE.pdf", options, true, true, true, true);
            reader.GetResult();
        }
    }
}
