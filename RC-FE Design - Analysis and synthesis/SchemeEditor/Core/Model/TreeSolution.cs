using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
{
    public class TreeSolution
    {
        public string Name { get; set; }
        public TreeProjects Projects { get; set; }

        public TreeSolution(string name, TreeProjects projects)
        {
            Name = name;
            Projects = projects;
        }
    }
}
