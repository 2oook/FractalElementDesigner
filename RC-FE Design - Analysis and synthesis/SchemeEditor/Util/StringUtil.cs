using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Util
{
    public static class StringUtil
    {
        public static bool Compare(string strA, string strB)
        {
            return string.Compare(strA, strB, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public static bool StartsWith(string str, string value)
        {
            return str.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
