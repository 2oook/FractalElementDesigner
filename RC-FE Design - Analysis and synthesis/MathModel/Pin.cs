using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Вывод элемента
    /// </summary>
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
