using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO
{
    public enum ResultType
    {
        Skipped = 1,
        Failed = 2,
        Read = 4,
        Encrypted = 8
    }
}
