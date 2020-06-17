using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public class ParseOptions
    {
        #region Properties

        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public bool AppendIds { get; set; }
        public bool UpdateIds { get; set; }
        public bool Select { get; set; }
        public bool CreateElements { get; set; }
        public DiagramProperties Properties { get; set; }
        public IdCounter Counter { get; set; } 

        #endregion
    } 
}
