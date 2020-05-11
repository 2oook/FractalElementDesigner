using GalaSoft.MvvmLight;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    public class Page404ViewModel : ViewModelBase, IPageViewModel
    {
        #region Команды

        /// <summary>
        /// Команда для перемещения на главную страницу 
        /// </summary>
        public ICommand GoToMainPageCommand { get; set; }

        #endregion
    }
}
