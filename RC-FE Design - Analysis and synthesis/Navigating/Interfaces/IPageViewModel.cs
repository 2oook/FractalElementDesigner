using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FractalElementDesigner.Navigating.Interfaces
{
    public interface IPageViewModel : INotifyPropertyChanged
    {
        void SetPage(Page page);

        ICommand GoToMainPageCommand { get; set; }
    }
}
