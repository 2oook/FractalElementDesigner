using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Вывод элемента
    /// </summary>
    [Serializable]
    class Pin
    {
        /// <summary>
        /// Номер вывода
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Ссылка на ячейку данного вывода
        /// </summary>
        public StructureCellBase Cell { get; set; }
    }
}
