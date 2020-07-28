﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Соединение
    /// </summary>
    class Connection
    {
        /// <summary>
        /// Секция 1
        /// </summary>
        public FESection FirstSection { get; set; }

        /// <summary>
        /// Секция 2
        /// </summary>
        public FESection SecondSection { get; set; }


        public int ConnectionType { get; set; } = 7;
    }
}
