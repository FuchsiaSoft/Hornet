using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing
{
    internal interface IContentReader
    {
        bool TryGetContent(string filePath, out string result);
    }
}
