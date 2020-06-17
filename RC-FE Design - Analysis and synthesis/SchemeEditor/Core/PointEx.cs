using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public class PointEx : IPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointEx(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
