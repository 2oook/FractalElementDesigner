using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Core.Model
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
