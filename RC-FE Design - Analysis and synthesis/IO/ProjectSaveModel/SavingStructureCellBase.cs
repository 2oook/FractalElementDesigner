using FractalElementDesigner.FEEditing.Model.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.IO.ProjectSaveModel
{
    /// <summary>
    /// Базовый класс ячейки структуры
    /// </summary>
    [Serializable]
    public class SavingStructureCellBase
    {
        private CellType cellType = CellType.None;
        /// <summary>
        /// Тип ячейки структуры
        /// </summary>
        public CellType CellType
        {
            get => cellType;
            set
            {
                cellType = value;
            }
        }
    }
}
