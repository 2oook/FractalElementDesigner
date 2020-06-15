using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Tools
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
