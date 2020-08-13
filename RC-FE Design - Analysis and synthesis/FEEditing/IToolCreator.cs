using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing
{
    interface IToolCreator
    {
        IEditingTool CreateTool(ToolType toolType);
    }
}
