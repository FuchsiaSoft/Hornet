using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing.ContentReaders
{
    class DocxContentReader : IContentReader
    {
        public bool TryGetContent(Stream fileStream, out string result)
        {
            result = string.Empty;
            if (fileStream.Length == 0) return false;

            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(fileStream, false))
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    result = sr.ReadToEnd();
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
