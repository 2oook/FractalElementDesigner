// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing
{
    public static class SnapHelper
    {
        public static double Snap(double original, double snap, double offset)
        {
            return Snap(original - offset, snap) + offset;
        }

        public static double Snap(double original, double snap)
        {
            return original + ((Math.Round(original / snap) - original / snap) * snap);
        }
    }
}
