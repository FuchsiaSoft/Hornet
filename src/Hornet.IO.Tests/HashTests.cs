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

        /// <summary>
        /// Tests to make sure that a hash is correct found in a zip file
        /// where the zip archive contains only the target file
        /// </summary>
        [TestMethod]
        public void Zip_Single_File_No_Nesting()
        {
            string knownFilePath = "TestFiles/Zips/Single_File_No_Nesting.zip";
            string knownMD5      = "97886269414575E12A8F6F92B1340100";
            string knownSHA1     = "A96C5DC23FF9CF79EEBB6BC181A6BACE582F2060";
            string knownSHA256   = "A9B5E893B658DBC08BAE3183DBA47324DE130D066C70DD1D7A93C9988646BAAC";

            ScanOptions options = new ScanOptions()
            {
                HashIncludeZip = true,
                MaxZipInMemorySize = 0,
                UnzipInMemory = true
            };

            FileReader reader = new FileReader(knownFilePath, options, true, true, true, false);
            FileResult result = reader.GetResult();

            Assert.IsTrue(result.EmbeddedResults.Count == 1);
            Assert.IsTrue(result.EmbeddedResults.First().MD5 == knownMD5);
            Assert.IsTrue(result.EmbeddedResults.First().SHA1 == knownSHA1);
            Assert.IsTrue(result.EmbeddedResults.First().SHA256 == knownSHA256);
        }

        /// <summary>
        /// Tests to make sure that a hash is correct found in a zip file
        /// where the zip archive contains the target file and some other non
        /// matched files
        /// </summary>
        [TestMethod]
        public void Zip_Multiple_Files_No_Nesting()
        {
            string knownFilePath = "TestFiles/Zips/Multiple_Files_No_Nesting.zip";
            string knownMD5 = "97886269414575E12A8F6F92B1340100";
            string knownSHA1 = "A96C5DC23FF9CF79EEBB6BC181A6BACE582F2060";
            string knownSHA256 = "A9B5E893B658DBC08BAE3183DBA47324DE130D066C70DD1D7A93C9988646BAAC";

            ScanOptions options = new ScanOptions()
            {
                HashIncludeZip = true,
                MaxZipInMemorySize = 0,
                UnzipInMemory = true
            };

            FileReader reader = new FileReader(knownFilePath, options, true, true, true, false);
            FileResult result = reader.GetResult();

            Assert.IsTrue(result.EmbeddedResults.Count > 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.MD5 == knownMD5) == 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.SHA1 == knownSHA1) == 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.SHA256 == knownSHA256) == 1);
        }

        /// <summary>
        /// Tests to make sure that a hash is correct found in a zip file
        /// where the zip archive contains the target file and some other non
        /// matched files, but the matched file is inside a nested zip
        /// </summary>
        [TestMethod]
        public void Zip_Multiple_Files_Nested_Once()
        {
            string knownFilePath = "TestFiles/Zips/Multiple_Files_Nested_Once.zip";
            string knownMD5 = "97886269414575E12A8F6F92B1340100";
            string knownSHA1 = "A96C5DC23FF9CF79EEBB6BC181A6BACE582F2060";
            string knownSHA256 = "A9B5E893B658DBC08BAE3183DBA47324DE130D066C70DD1D7A93C9988646BAAC";

            ScanOptions options = new ScanOptions()
            {
                HashIncludeZip = true,
                MaxZipInMemorySize = 0,
                UnzipInMemory = true
            };

            FileReader reader = new FileReader(knownFilePath, options, true, true, true, false);
            FileResult result = reader.GetResult();

            Assert.IsTrue(result.EmbeddedResults.Count > 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.MD5 == knownMD5) == 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.SHA1 == knownSHA1) == 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.SHA256 == knownSHA256) == 1);
        }

        /// <summary>
        /// Tests to make sure that a hash is correct found in a zip file
        /// where the zip archive contains the target file and some other non
        /// matched files, where the target file is in a directory
        /// </summary>
        [TestMethod]
        public void Zip_Multiple_Files_And_Folders_No_Nesting()
        {
            string knownFilePath = "TestFiles/Zips/Multiple_Files_And_Folders_No_Nesting.zip";
            string knownMD5 = "97886269414575E12A8F6F92B1340100";
            string knownSHA1 = "A96C5DC23FF9CF79EEBB6BC181A6BACE582F2060";
            string knownSHA256 = "A9B5E893B658DBC08BAE3183DBA47324DE130D066C70DD1D7A93C9988646BAAC";

            ScanOptions options = new ScanOptions()
            {
                HashIncludeZip = true,
                MaxZipInMemorySize = 0,
                UnzipInMemory = true
            };

            FileReader reader = new FileReader(knownFilePath, options, true, true, true, false);
            FileResult result = reader.GetResult();

            Assert.IsTrue(result.EmbeddedResults.Count > 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.MD5 == knownMD5) == 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.SHA1 == knownSHA1) == 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.SHA256 == knownSHA256) == 1);
        }

        /// <summary>
        /// Tests to make sure that the reading performs correctly when there
        /// are several layers of nesting in zips
        /// </summary>
        [TestMethod]
        public void Zip_Lots_Of_Nesting()
        {
            string knownFilePath = "TestFiles/Zips/Lots_Of_Nesting.zip";
            string knownMD5 = "97886269414575E12A8F6F92B1340100";
            string knownSHA1 = "A96C5DC23FF9CF79EEBB6BC181A6BACE582F2060";
            string knownSHA256 = "A9B5E893B658DBC08BAE3183DBA47324DE130D066C70DD1D7A93C9988646BAAC";

            ScanOptions options = new ScanOptions()
            {
                HashIncludeZip = true,
                MaxZipInMemorySize = 0,
                UnzipInMemory = true
            };

            FileReader reader = new FileReader(knownFilePath, options, true, true, true, false);
            FileResult result = reader.GetResult();

            Assert.IsTrue(result.EmbeddedResults.Count > 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.MD5 == knownMD5) == 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.SHA1 == knownSHA1) == 1);
            Assert.IsTrue(result.EmbeddedResults.Count(e => e.SHA256 == knownSHA256) == 1);
        }
    }
}
