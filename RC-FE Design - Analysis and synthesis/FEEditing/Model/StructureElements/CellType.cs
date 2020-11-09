using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model.StructureElements
{
    /// <summary>
    /// Перечисление тип ячейки
    /// </summary>
    [Flags]
    public enum CellType
    {
        None = 0,
        PlaceForContact = 1,
        Contact = 3,
        Forbid = 7,
        Shunt = 15,
        Cut = 2,
        RC = 6,
        R = 14
    }
}
