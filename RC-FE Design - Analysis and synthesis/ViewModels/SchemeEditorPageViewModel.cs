using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model;
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
            InitializeCommands();
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

        private ICommand testCommand;
        /// <summary>
        /// Команда 
        /// </summary>
        public ICommand TestCommand
        {
            get
            {
                return testCommand;
            }
            set
            {
                testCommand = value;
                RaisePropertyChanged(nameof(TestCommand));
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            TestCommand = new RelayCommand(Test);
        }

        private void Test() 
        {
            var canvas = _Page.Editor.Context.CurrentCanvas;

            var elements = canvas.GetElements();

            foreach (var element in elements)
            {
                //element.SetSelected(true);

                if (element.GetElementType() is Wire wire)
                {
                    element.SetSelected(true);
                }
            }
        }

        /// <summary>
        /// Метод для установки страницы
        /// </summary>
        /// <param name="page"></param>
        public void SetPage(Page page)
        {
            _Page = (SchemeEditorPage)page;
        }

        #endregion
    }
}
