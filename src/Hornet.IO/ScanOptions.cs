using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using File = Pri.LongPath.File;

namespace Hornet.IO
{
    /// <summary>
    /// Object for containing all scan options to be provided
    /// to the <see cref="HornetScanManager"/> on
    /// <see cref="HornetScanManager.StartScan(ScanOptions)"/>
    /// </summary>
    public class ScanOptions
    {
        /// <summary>
        /// The credentials to use for impersonation if required
        /// </summary>
        [JsonIgnore]
        public NetworkCredential Credentials { get; set; }

        public bool RegexIncludePdf { get; set; } = false;
        public bool RegexIncludeText { get; set; } = false;
        public bool RegexIncludeMSG { get; set; } = false;
        public bool RegexIncludeWord { get; set; } = false;
        public bool RegexIncludeExcel { get; set; } = false;
        public bool RegexIncludeHTML { get; set; } = false;

        public bool HashIncludeJpegInPdf { get; set; } = false;

        public IEnumerable<string> ExcludedExtensionsForTextAttempt { get; set; } = new string[0];
        /// <summary>
        /// The path to serve as root directory for the scan
        /// </summary>
        [JsonIgnore]
        public string RootDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of worker threads to use,
        /// any number less than 1 is treated as 1
        /// </summary>
        public int MaxWorkerThreads { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum file queue size, zero or any
        /// non positive int is counted as unlimited
        /// </summary>
        public int MaxFileQueueSize { get; set; } = 0;

        /// <summary>
        /// The timeout in seconds to apply to regex's
        /// </summary>
        public int RegexTimeoutSeconds { get; set; } = 0;

        /// <summary>
        /// Gets or sets a <see cref="long"/> representing the maxumum length
        /// of a file to attempt hashing.  Zero is treated as unlimited.  Default is 0
        /// </summary>
        public long MaxSizeToAttemptHash { get; set; } = 0;

        /// <summary>
        /// Gets or sets a <see cref="bool"/> indicating whether or not to attempt
        /// unpacking archive files and hashing their internals.  Default false
        /// </summary>
        public bool IncludeZip { get; set; } = false;

        /// <summary>
        /// Gets or sets a <see cref="bool"/> indicating whether or not to hold
        /// an entire file in memory before attempting multiple checks.  Default true
        /// </summary>
        public bool HoldBufferInMemory { get; set; } = true;

        /// <summary>
        /// Gets or sets a <see cref="long"/> representing the maximum length of a file
        /// to hold fully in memory if <see cref="HoldBufferInMemory"/> is set to true.
        /// Zero is treated as unlimited.  Default is 0
        /// </summary>
        public long InMemoryFileSizeLimit { get; set; } = 0;

        /// <summary>
        /// Gets or sets a <see cref="bool"/> indicating whether or not to un-pack
        /// archive files in-memory without touching local disk if <see cref="IncludeZip"/>
        /// is true.  Default true
        /// </summary>
        public bool UnzipInMemory { get; set; } = true;

        /// <summary>
        /// Gets or sets a <see cref="long"/> representing the maximum file size
        /// to attempt in-memory unpacking of archives if <see cref="UnzipInMemory"/>
        /// is set to true.  Zero is treated as unlimited, default 0
        /// </summary>
        public long MaxZipInMemorySize { get; set; } = 0;

        /// <summary>
        /// Gets or sets a <see cref="bool"/> indicating whether or not to unzip 
        /// archive files to local disk if <see cref="IncludeZip"/> is true,
        /// <see cref="UnzipInMemory"/> is true and <see cref="MaxZipInMemorySize"/>
        /// is set to 0.  Default false
        /// </summary>
        public bool UnzipToDiskIfTooBig { get; set; } = false;

        /// <summary>
        /// Gets or sets a <see cref="bool"/> indicating whether or not to enumerate
        /// the entire file system in the background whilst also working.
        /// This will enable overall progress tracking once enumeration has
        /// finished
        /// </summary>
        public bool BackgroundEnumerate { get; set; } = true;

        /// <summary>
        /// Gets or sets a <see cref="bool"/> indicating whether or not to build
        /// a list of files that were found to be encrypted
        /// </summary>
        public bool ListEncryptedFiles { get; set; } = true;

        /// <summary>
        /// Gets or sets a <see cref="bool"/> indicating whether or not to
        /// always attempt text decoding if file can not otherwise
        /// be read
        /// </summary>
        public bool AttemptTextDecode { get; set; } = true;

        /// <summary>
        /// Gets or sets a <see cref="long"/> representing the maximum
        /// length of a file to attempt text decoding
        /// </summary>
        public long MaxSizeToTextDecode { get; set; } = 0;

        [JsonConverter(typeof(StringEnumConverter))]
        public EncodingType EncodingType { get; set; }

        /// <summary>
        /// The list of <see cref="HashInfoGroup"/> objects to match against
        /// </summary>
        public List<HashInfoGroup> HashGroups { get; set; } = new List<HashInfoGroup>();

        /// <summary>
        /// The list of <see cref="RegexInfoGroup"/> objects to match against
        /// </summary>
        public List<RegexInfoGroup> RegexGroups { get; set; } = new List<RegexInfoGroup>();

        public bool SaveToFile(string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void ReadFromFile(string filePath)
        {

        }
    }
}
