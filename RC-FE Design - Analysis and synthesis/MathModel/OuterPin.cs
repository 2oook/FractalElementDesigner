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
        public int Number { get; set; }

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