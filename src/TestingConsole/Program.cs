using Hornet.IO;
using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string dirPath = "C:\\tmp\\";
            string fileName = "Example.pdf";

            PdfDocument pdf = PdfReader.Open(dirPath + fileName);
            pdf.Save(dirPath + "PDF_14_Example.pdf");
            pdf.Version = 17;
            pdf.Save(dirPath + "PDF_17_Example.pdf");
        }
    }
}
