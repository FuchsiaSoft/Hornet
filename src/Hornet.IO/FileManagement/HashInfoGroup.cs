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
        public List<HashResult> Matches { get; internal set; } = new List<HashResult>();

        public Stream MakeDefinitionFile()
        {
            //TODO: make a zip file in memory here
            //have a stream writer write the contents of this object
            //serialized, and then pass that stream to ionic
            throw new NotImplementedException();
        }
    }
}
