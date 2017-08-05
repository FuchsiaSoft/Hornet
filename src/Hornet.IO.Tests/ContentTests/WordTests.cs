using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.Tests.ContentTests
{
    [TestClass]
    public class WordTests
    {
        [TestMethod]
        public void OpenOffice_DOCX_Test()
        {
            string path = "TestFiles/Word/OpenOffice_DOCX_Example.docx";
            ScanOptions options = new ScanOptions();
            FileReader reader = new FileReader(path, options, false, false, false, true);
            FileResult result = reader.GetResult();

            Assert.IsTrue(TestUtil.KnownSingleMatchPattern.Matches(result.Content).Count == 1);
            Assert.IsTrue(TestUtil.KnownMultiMatchPattern.Matches(result.Content).Count > 1);
        }
    }
}
