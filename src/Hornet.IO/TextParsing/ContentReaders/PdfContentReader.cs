using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

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
                using (PdfReader reader = new PdfReader(fileStream))
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        sb.Append(PdfTextExtractor.GetTextFromPage(reader, i + 1));
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
