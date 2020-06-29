using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public interface ISelected
    {
        bool GetSelected();
        void SetSelected(bool selected);
    }
}
