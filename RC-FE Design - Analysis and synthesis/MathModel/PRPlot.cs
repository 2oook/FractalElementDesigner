using OxyPlot;
using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// График
    /// </summary>
    public class PRPlot : IProjectTreeItem
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; } = "График";

        public PlotModel Model { get; set; }

        public List<List<double>> Points { get; set; }
    }
}
