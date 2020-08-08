using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Core
{
    public interface IData
    {
        object GetData();
        void SetData(object data);
    }
}
