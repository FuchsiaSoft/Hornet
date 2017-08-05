using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Content.Objects;

namespace Hornet.IO.TextParsing.ContentReaders
{
    internal class PdfContentReader : IContentReader
    {
        public bool TryGetContent(Stream fileStream, out string result)
        {
            result = string.Empty;
            if (fileStream.Length == 0) return false;

            try
            {
                using (PdfDocument pdf = PdfReader.Open(fileStream))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (PdfPage page in pdf.Pages)
                    {
                        foreach (string subString in page.ExtractText())
                        {
                            sb.Append(subString);
                        }
                    }
                    result = sb.ToString();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        
    }
}
