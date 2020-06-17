using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
{
    public class TreeSolution
    {
        public string Name { get; set; }
        public string TagFileName { get; set; }
        public string TableFileName { get; set; }
        public TreeProjects Projects { get; set; }

        public TreeSolution(string name, string tagFileName, string tableFileName, TreeProjects projects)
        {
            Name = name;
            TagFileName = tagFileName;
            TableFileName = tableFileName;
            Projects = projects;
        }
    }
}
