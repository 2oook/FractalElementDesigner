using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells
{
    /// <summary>
    /// Базовый класс ячейки структуры
    /// </summary>
    public class StructureCellBase
    {
        /// <summary>
        /// Тип ячейки структуры
        /// </summary>
        public CellType CellType { get; set; } = CellType.None;
    }
}
