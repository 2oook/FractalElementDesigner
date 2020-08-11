using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Editor
{
    public static class Constants
    {
        public const char TagNameSeparator = '|';

        public const string TagHeaderScheme = "Scheme";

        public const string TagElementPin = "Pin";
        public const string TagElementWire = "Wire";
        public const string TagElementFElement = "FElement";
        public const string TagElementTopGround = "TopGround";
        public const string TagElementBottomGround = "BottomGround";

        public const string WireStartType = "Start";
        public const string WireEndType = "End";

        public const string PinStandalone = "MiddlePin";
    }
}
