using GetDocText.Doc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hornet.IO.TextParsing.ContentReaders
{
    internal class BinaryWordDocContentReader : IContentReader
    {
        public bool TryGetContent(Stream fileStream, out string result)
        {
            //TODO: implement better binary word doc reader that doesn't
            //only work from disk using native methods
            throw new InvalidOperationException("Cannot read binary word docs from stream");
        }

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
