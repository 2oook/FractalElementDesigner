using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
{
    public class Connection
    {
        public IElement Element { get; set; }
        public List<Wire> Wires { get; set; }

        public Connection(IElement element, List<Wire> wires)
        {
            Element = element;
            Wires = wires;
        }
    }
}
