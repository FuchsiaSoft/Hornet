using Hornet.IO;
using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System;
using System.Collections.Generic;
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
                Credentials = new System.Net.NetworkCredential("username", "password", "domain"),
                EncodingType = Hornet.IO.TextParsing.EncodingType.AutoDetect,
                HoldBufferInMemory = true,
                IncludeZip = true,
                InMemoryFileSizeLimit = 20971520,
                MaxFileQueueSize = 20000,
                MaxSizeToAttemptHash = 1073741824,
                MaxWorkerThreads = 8,
                MaxZipInMemorySize = 524288000,
                RegexTimeoutSeconds = 30,
                UnzipInMemory = true,
                UnzipToDiskIfTooBig = false,
                HashGroups = new List<HashInfoGroup>()
                {
                    new HashInfoGroup()
                    {
                        Name = "Category 1 files",
                        Description = "Example of a hash group",
                        MD5s = new List<HashInfo>
                        {
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" },
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" },
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" },
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" },
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" }
                        }
                    },
                    new HashInfoGroup()
                    {
                        Name = "Category 2 files",
                        Description = "Example of a hash group",
                        MD5s = new List<HashInfo>
                        {
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" },
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" },
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" },
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" },
                            new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks="test hash" }
                        }
                    }
                },
                RegexGroups = new List<Hornet.IO.TextParsing.RegexInfoGroup>()
                {
                    new Hornet.IO.TextParsing.RegexInfoGroup()
                    {
                        Name = "Key words group 1",
                        Description = "example of a key words group",
                        RegexInfos = new List<RegexInfo>
                        {
                            new RegexInfo() { Pattern = "(?i)(Some(where|one|thing))" },
                            new RegexInfo() { Pattern = @"(?:\d*\.)?\d+" }
                        }
                    },
                    new Hornet.IO.TextParsing.RegexInfoGroup()
                    {
                        Name = "Key words group 1",
                        Description = "example of a key words group",
                        RegexInfos = new List<RegexInfo>
                        {
                            new RegexInfo() { Pattern = "(?i)(Some(where|one|thing))" },
                            new RegexInfo() { Pattern = @"(?:\d*\.)?\d+" }
                        }
                    }
                }
            };



            for (int i = 0; i < 400000; i++)
            {
                options.HashGroups.First().MD5s.Add(new HashInfo() { Hash = "A22356FD8F2F08AEFC2871E8113AC91F", HashType = HashType.MD5, Remarks = "test hash" });
            }

            for (int i = 0; i < 2000; i++)
            {
                options.RegexGroups.First().RegexInfos.Add(new RegexInfo() { Pattern = "(?i)(Some(where|one|thing))" });
                options.RegexGroups.First().RegexInfos.Add(new RegexInfo() { Pattern = @"(?:\d*\.)?\d+" });
            }

            options.SaveToFile("StandardSearch.HSEC");
        }
    }
}
