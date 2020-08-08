using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Core.Model
{
    public class Pin
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public Pin(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
