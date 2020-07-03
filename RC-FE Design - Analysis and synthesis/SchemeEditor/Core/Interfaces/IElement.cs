using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
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
