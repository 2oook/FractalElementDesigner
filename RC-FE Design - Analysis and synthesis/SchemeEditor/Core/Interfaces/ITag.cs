using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Core
{
    public interface ITag
    {
        object GetTag();
        void SetTag(object type);
    }
}
