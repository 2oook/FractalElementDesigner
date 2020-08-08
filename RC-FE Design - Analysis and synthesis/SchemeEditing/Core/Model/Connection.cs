using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Core.Model
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
