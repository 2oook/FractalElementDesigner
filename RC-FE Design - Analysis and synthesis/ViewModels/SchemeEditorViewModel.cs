using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    public class SchemeEditorViewModel : IPageViewModel
    {
        public SchemeEditorViewModel()
        {
                
        }

        public SchemeEditorViewModel(IDialogCoordinator dialogCoordinator) : this()
        {
            _dialogCoordinator = dialogCoordinator;
        }

        /// <summary>
        /// Объект для вывода диалогов
        /// </summary>
        private IDialogCoordinator _dialogCoordinator;

        public ICommand GoToMainPageCommand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetPage(Page page)
        {
            throw new NotImplementedException();
        }
    }
}
