using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditor.Tools
{
    /// <summary>
    /// Перечисление тип инструмента
    /// </summary>
    public enum ToolType
    {
        None,
        ContactNumerator,
        CutCellDisposer,
        ContactCellDisposer,
        ForbidContactDisposer,
        RCCellDisposer,
        RCellDisposer,
        ShuntCellDisposer
    }
}
