﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces
{
    public interface IPageResolver
    {
        Page GetPageInstance(string alias);
    }
}
