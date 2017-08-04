using Hornet.IO.FileManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.Tests
{
    [TestClass]
    public class HashTests
    {
        /// <summary>
        /// Tests to make sure the output string format for hashes is
        /// correct against a known file to ensure that we don't
        /// inadvertantly make any changes to the output that will
        /// break with conventional way of writing hashes (e.g. using
        /// .Net's BitConverter)
        /// </summary>
        [TestMethod]
        public void Verify_Hash_Output_Strings()
        {
            string knownFilePath = "TestFiles/HashOutputSampleFile.pdf";
            string knownMD5      = "97886269414575E12A8F6F92B1340100";
            string knownSHA1     = "A96C5DC23FF9CF79EEBB6BC181A6BACE582F2060";
            string knownSHA256   = "A9B5E893B658DBC08BAE3183DBA47324DE130D066C70DD1D7A93C9988646BAAC";

            FileReader reader = new FileReader(knownFilePath, new ScanOptions(), true, true, true, false);
            FileResult result = reader.GetResult();

            Assert.IsTrue(result.MD5 == knownMD5);
            Assert.IsTrue(result.SHA1 == knownSHA1);
            Assert.IsTrue(result.SHA256 == knownSHA256);
        }


    }
}
