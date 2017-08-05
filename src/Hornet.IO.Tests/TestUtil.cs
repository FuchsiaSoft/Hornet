using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hornet.IO.Tests
{
    internal class TestUtil
    {
        public static Regex KnownSingleMatchPattern { get; set; } = 
            new Regex("(?i)(conversation on political matters)");

        public static Regex KnownMultiMatchPattern { get; set; } = 
            new Regex("Austria");
    }
}
