using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Соединение
    /// </summary>
    [Serializable]
    class Connection
    {
        /// <summary>
        /// Секция 1
        /// </summary>
        public FESection FirstSection;

        /// <summary>
        /// Секция 2
        /// </summary>
        public FESection SecondSection;

        /// <summary>
        /// Тип соединения двух секций
        /// </summary>
        public int ConnectionType = 7;

        /// <summary>
        /// Тип заземления
        /// </summary>
        public int PEType = 1;
    }
}
