using System;
using System.Collections.Generic;
using System.Linq;

namespace FractalElementDesigner.SchemeEditing.Core
{
    public interface IUid
    {
        string GetUid();
        void SetUid(string uid);
    }
}
