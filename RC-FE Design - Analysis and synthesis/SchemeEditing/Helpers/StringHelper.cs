// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace FractalElementDesigner.SchemeEditing
{
    public static class StringHelper
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
