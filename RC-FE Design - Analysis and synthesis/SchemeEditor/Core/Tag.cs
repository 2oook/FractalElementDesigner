using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public class Tag
    {
        #region Properties

        public int Id { get; set; }

        public string Designation { get; set; }
        public string Signal { get; set; }
        public string Condition { get; set; }
        public string Description { get; set; }

        #endregion
    } 
}
