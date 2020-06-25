using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces
{
    public interface IPageViewModel : INotifyPropertyChanged
    {
        void SetPage(Page page);
    }
}
