﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public interface ITag
    {
        object GetTag();
        void SetTag(object tag);
    }
}