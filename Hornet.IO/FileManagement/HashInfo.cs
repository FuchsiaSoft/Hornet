using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.FileManagement
{
    /// <summary>
    /// Provides a container for a hash to be matched
    /// and remarks for it to be included in report
    /// </summary>
    public class HashInfo
    {
        /// <summary>
        /// The hash value, to be set as hex encoded
        /// with no separating characters
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// The type of Hash
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public HashType HashType { get; set; }

        /// <summary>
        /// Remarks for the hash
        /// </summary>
        public string Remarks { get; set; }
    }
}
