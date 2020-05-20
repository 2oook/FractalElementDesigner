using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Model
{
    public class RCStructure : RCStructureBase
    {
        public StructureCellBase[,] StructureCells { get; set; } = null;
    }
}
