using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Вывод элемента
    /// </summary>
    [Serializable]
    public class Pin
    {
        /// <summary>
        /// Номер вывода
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Ссылка на секцию данного вывода
        /// </summary>
        public FESection Section { get; set; }
    }
}
