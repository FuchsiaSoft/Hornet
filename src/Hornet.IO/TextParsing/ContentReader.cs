using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing
{
    internal abstract class ContentReader : IContentReader
    {
        
        public abstract bool TryGetContent(Stream fileStream, out string result);
    }
}
