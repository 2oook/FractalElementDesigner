using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
{
    public class TreeProject
    {
        public string Name { get; set; }
        public TreeDiagrams Diagrams { get; set; }

        public TreeProject(string name, TreeDiagrams diagrams)
        {
            Name = name;
            Diagrams = diagrams;
        }
    }
}
