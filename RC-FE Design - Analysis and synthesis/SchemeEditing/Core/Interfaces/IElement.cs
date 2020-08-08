using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Core
{
    public interface IElement : IData, IUid, ITag, ISelected
    {
        double GetX();
        double GetY();
        void SetX(double x);
        void SetY(double y);

        object GetParent();

        ElementType ElementType { get; set; }
    }
}
