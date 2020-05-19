using RC_FE_Design___Analysis_and_synthesis.FEEditor.Controls;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor
{
    public class Context
    {
        #region Properties


        #endregion

        #region Fields

        public FECanvas CurrentCanvas = null;

        public double DefaultGridSize = 30;

        public bool EnableSnap = true;
        public bool SnapOnRelease = false;

        public bool SkipContextMenu = false;
        public bool SkipLeftClick = false;

        public PointEx PanStart;
        public double PreviousScrollOffsetX = -1.0;
        public double PreviousScrollOffsetY = -1.0;

        public double ZoomInFactor = 0.1;
        public double ZoomOutFactor = 0.1;

        public double ZoomLogBase = 1.9;
        public double ZoomExpFactor = 1.3;

        public PointEx ZoomPoint;

        public double ReversePanDirection = -1.0; // reverse: 1.0, normal: -1.0


        #endregion
    }
}
