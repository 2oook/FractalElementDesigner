using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model.Cells
{
    /// <summary>
    /// Перечисление тип ячейки
    /// </summary>
    public enum CellType
    {
        None,
        PlaceForContact,
        Contact,
        Cut,
        Forbid,
        RC,
        R,
        Shunt
    }
}
