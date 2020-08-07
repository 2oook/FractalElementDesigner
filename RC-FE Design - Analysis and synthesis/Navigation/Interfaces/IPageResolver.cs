using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FractalElementDesigner.Navigating.Interfaces
{
    public interface IPageResolver
    {
        Page GetPageInstance(string alias);
    }
}
