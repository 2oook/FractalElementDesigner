﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    class AllowablePinsConnection
    {
        public int[,] ConnectionMatrix { get; set; }

        public Dictionary<int, int[]> PEVector { get; set; }
    }
}
