using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor
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
