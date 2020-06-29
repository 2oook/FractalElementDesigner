using GalaSoft.MvvmLight;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
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
    /// <summary>
    /// ViewModel страницы редактирования схем включения
    /// </summary>
    public class SchemeEditorPageViewModel : ViewModelBase, IPageViewModel
    {
        public SchemeEditorPageViewModel()
        {
                
        }

        public SchemeEditorPageViewModel(IDialogCoordinator dialogCoordinator) : this()
        {
            _dialogCoordinator = dialogCoordinator;
        }

        #region Глобальные переменные


        /// <summary>
        /// Ссылка на страницу
        /// </summary>
        private SchemeEditorPage _Page { get; set; }

        /// <summary>
        /// Объект для вывода диалогов
        /// </summary>
        private IDialogCoordinator _dialogCoordinator;

        #endregion

        #region Свойства



        #endregion

        #region Команды



        #endregion

        #region Методы

        private void Test() 
        {
            //_Page.Editor
        }

        /// <summary>
        /// Метод для установки страницы
        /// </summary>
        /// <param name="page"></param>
        public void SetPage(Page page)
        {
            _Page = (SchemeEditorPage)page;

            Test();
        }

        #endregion
    }
}
