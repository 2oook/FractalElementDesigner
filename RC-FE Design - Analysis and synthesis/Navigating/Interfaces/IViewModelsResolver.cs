using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.Navigating.Interfaces
{
    public interface IViewModelsResolver
    {
        IPageViewModel GetViewModelInstance(string alias);
    }
}
