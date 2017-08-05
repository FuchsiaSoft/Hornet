using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.Tests.ContentTests
{
    [TestClass]
    public class PdfTests
    {
        [TestMethod]
        public void Pdf_14_Test_Match()
        {
            string path = "TestFiles/Pdf/PDF_14_Example.pdf";
            ScanOptions options = new ScanOptions();
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }

        [TestMethod]
        public void Pdf_17_Test_Match()
        {
            string path = "TestFiles/Pdf/PDF_17_Example.pdf";
            ScanOptions options = new ScanOptions();
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }
    }
}
