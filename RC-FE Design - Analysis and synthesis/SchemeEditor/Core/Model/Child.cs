using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
{
    public class Child
    {
        public object Element { get; set; }
        public List<Pin> Pins { get; set; }

        public Child(object element, List<Pin> pins)
        {
            Element = element;
            Pins = pins;
        }
    }
}
