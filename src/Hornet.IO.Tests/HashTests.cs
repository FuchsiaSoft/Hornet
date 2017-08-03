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
            string knownFilePath = "TestFiles/Text/HashOutputSampleFile.txt";
            string knownMD5      = "B64F2AA367438799A59E88A66DA1BF61";
            string knownSHA1     = "7A4CE47EB34024DC2B1D9B40FCE7ACCFAB3FAA83";
            string knownSHA256   = "4A23F5587B493C39173287E77F0912C55CAB9F26305B80F4E8142AFEEA0C6808";

            FileReader reader = new FileReader(knownFilePath, new ScanOptions(), true, true, true, false);
            FileResult result = reader.GetResult();

            Assert.IsTrue(result.MD5 == knownMD5);
            Assert.IsTrue(result.SHA1 == knownSHA1);
            Assert.IsTrue(result.SHA256 == knownSHA256);
        }


    }
}
