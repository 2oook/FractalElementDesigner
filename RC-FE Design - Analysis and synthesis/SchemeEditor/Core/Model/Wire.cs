using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
{
    public class Wire
    {
        public object Line { get; set; }
        public IElement Start { get; set; }
        public IElement End { get; set; }

        public Wire(object line, IElement start, IElement end)
        {
            Line = line;
            Start = start;
            End = end;
        }
    }
}
