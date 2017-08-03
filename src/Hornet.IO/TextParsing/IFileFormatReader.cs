using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing
{
    internal interface IFileFormatReader
    {
        bool TryGetContent(string filePath, out string result);
        bool TryGetContent(Stream fileStream, out string result);
    }
}
