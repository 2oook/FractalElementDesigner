using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    public class Connection
    {
        public List<FESection> Sections { get; set; }

        public int[] SchemeIndices { get; set; } = { 1, 2 }; 
    }
}
