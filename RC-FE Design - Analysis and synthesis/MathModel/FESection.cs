using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    public class FESection
    {
        public FESection(FESectionParameters sectionParameters)
        {
            SectionParameters = sectionParameters;
        }

        public FESectionParameters SectionParameters { get; set; }

        public int[] SchemeIndices { get; set; } = { 0, 0 };
    }
}
