using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
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
