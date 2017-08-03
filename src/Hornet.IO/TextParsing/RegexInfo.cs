using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing
{
    public class RegexInfo
    {
        private static string _sampleString = 
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Cras eget tortor nec risus gravida maximus.";

        private Regex _regex = null;

        private bool? _isValid;

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether <see cref="RegexInfo.Pattern"/>
        /// contains a valid Regular Expression.  Only evaluated once per object instance
        /// then persisted
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_isValid != null) return (bool)_isValid;
                _isValid = TestPattern();
                return (bool)_isValid;
            }
        }

        public string Pattern { get; set; }

        private bool TestPattern()
        {
            if (string.IsNullOrWhiteSpace(Pattern)) return false;

            try
            {
                AsRegex().IsMatch(_sampleString);
            }
            catch (Exception)
            {
                return false;
            }

            return true;            
        }

        public Regex AsRegex()
        {
            if (_regex == null)
            {
                _regex = new Regex(Pattern, RegexOptions.Compiled);
            }

            return _regex;
        }
    }
}
