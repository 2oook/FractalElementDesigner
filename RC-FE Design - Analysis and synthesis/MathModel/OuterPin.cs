using System;
using System.Collections.Generic;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Внешний вывод
    /// </summary>
    [Serializable]
    class OuterPin
    {
        /// <summary>
        /// Состояние вывода
        /// </summary>
        public OuterPinState State { get; set; }
    }

    /// <summary>
    /// Состояние внешнего вывода
    /// </summary>
    enum OuterPinState 
    {
        Gnd,
        In,
        Float,
        Con
    }
}