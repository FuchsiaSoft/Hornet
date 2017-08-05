using Hornet.IO.TextParsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hornet.IO.Tests.ContentTests
{
    /// <summary>
    /// Tests to make sure that text is decoding properly.  The focus of these tests is not
    /// on the regex's, just making sure that the reader is consistently decoding
    /// text correctly.
    /// </summary>
    [TestClass]
    public class TextTests
    {
        

        [TestMethod]
        public void ASCII_Test_Match()
        {
            string path = "TestFiles/Text/ASCII_Example.txt";
            ScanOptions options = new ScanOptions() { EncodingType = EncodingType.ASCII };
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }

        [TestMethod]
        public void Unicode_Test_Match()
        {
            string path = "TestFiles/Text/Unicode_Example.txt";
            ScanOptions options = new ScanOptions() { EncodingType = EncodingType.Unicode };
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }

        [TestMethod]
        public void UnicodeBE_Test_Match()
        {
            string path = "TestFiles/Text/UnicodeBE_Example.txt";
            ScanOptions options = new ScanOptions() { EncodingType = EncodingType.UnicodeBigEndian };
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }

        [TestMethod]
        public void UTF32_Test_Match()
        {
            string path = "TestFiles/Text/UTF32_Example.txt";
            ScanOptions options = new ScanOptions() { EncodingType = EncodingType.UTF32 };
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }

        [TestMethod]
        public void UTF7_Test_Match()
        {
            string path = "TestFiles/Text/UTF7_Example.txt";
            ScanOptions options = new ScanOptions() { EncodingType = EncodingType.UTF7 };
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }

        [TestMethod]
        public void UTF8_Test_Match()
        {
            string path = "TestFiles/Text/UTF8_Example.txt";
            ScanOptions options = new ScanOptions() { EncodingType = EncodingType.UTF8 };
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }
    }
}
