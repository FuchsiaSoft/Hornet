using GetDocText.Doc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing.ContentReaders
{
    internal class BinaryWordDocContentReader : IFileFormatReader
    {
        public bool TryGetContent(string filePath, out string result)
        {
            try
            {
                TextLoader loader = new TextLoader(filePath);
                return loader.LoadText(out result);
            }
            catch (Exception)
            {
                //TODO: implement logging
                result = string.Empty;
                return false;
            }
        }
    }
}
