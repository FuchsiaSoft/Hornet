﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing
{
    public enum EncodingType
    {
        ASCII = 1,
        UTF7 = 2,
        UTF8 = 4,
        Unicode = 8,
        UnicodeBigEndian = 16,
        UTF32 = 32,
        AutoDetect = 64
    }
}
