using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public class DiagramProperties
    {
        #region Properties

        public int PageWidth { get; set; }
        public int PageHeight { get; set; }
        public int GridOriginX { get; set; }
        public int GridOriginY { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public int GridSize { get; set; }
        public double SnapX { get; set; }
        public double SnapY { get; set; }
        public double SnapOffsetX { get; set; }
        public double SnapOffsetY { get; set; }

        #endregion

        #region Defaults

        public static DiagramProperties Default
        {
            get
            {
                return new DiagramProperties()
                {
                    PageWidth = 1260,
                    PageHeight = 891,
                    GridOriginX = 0,
                    GridOriginY = 0,
                    GridWidth = 1260,
                    GridHeight = 891,
                    GridSize = 30,
                    SnapX = 15,
                    SnapY = 15,
                    SnapOffsetX = 0,
                    SnapOffsetY = 0
                };
            }
        }

        #endregion
    } 
}
