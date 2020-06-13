using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells
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
