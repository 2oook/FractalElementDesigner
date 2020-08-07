﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Состояние особи в ГА
    /// </summary>
    [Serializable]
    class StateOfIndividual
    {
        /// <summary>
        /// Оценка особи
        /// </summary>
        public int Rate { get; set; }
    }
}
