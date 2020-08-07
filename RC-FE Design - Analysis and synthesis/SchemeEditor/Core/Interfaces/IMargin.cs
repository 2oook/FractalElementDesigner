using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Core
{
    public interface IMargin
    {
        double Bottom { get; set; }
        double Left { get; set; }
        double Right { get; set; }
        double Top { get; set; }
    }
}
