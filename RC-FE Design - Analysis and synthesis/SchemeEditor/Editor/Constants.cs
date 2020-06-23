using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Editor
{
    public static class Constants
    {
        public const char ArgumentSeparator = ';';
        public const string PrefixRoot = "+";
        public const string PrefixChild = "-";

        public const char TagNameSeparator = '|';

        public const string TagHeaderSolution = "Solution";
        public const string TagHeaderProject = "Project";
        public const string TagHeaderDiagram = "Diagram";

        public const string TagElementPin = "Pin";
        public const string TagElementWire = "Wire";
        public const string TagElementAndGate = "AndGate";
        public const string TagElementFElement = "FElement";

        public const string WireStartType = "Start";
        public const string WireEndType = "End";

        public const string PinStandalone = "MiddlePin";
        public const string PinLeft = "LeftPin";
        public const string PinRight = "RightPin";
        public const string PinTop = "TopPin";
        public const string PinBottom = "BottomPin";
    }
}
