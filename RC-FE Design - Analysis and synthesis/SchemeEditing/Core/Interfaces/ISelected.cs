using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Core
{
    public interface ISelected
    {
        bool GetSelected();
        void SetSelected(bool selected);
    }
}
