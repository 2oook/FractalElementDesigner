using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Controls;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Editor;
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

        private ICanvas currentCanvas;
        private ICanvas _CurrentCanvas 
        { 
            get => currentCanvas;
            set 
            {
                currentCanvas = value;

                if (currentCanvas.ElementAdded == null)
                {
                    currentCanvas.ElementAdded = ElementAddedHandler;
                }
            }
        }

        /// <summary>
        /// Объект для вывода диалогов
        /// </summary>
        private IDialogCoordinator _dialogCoordinator;

        #endregion

        #region Свойства



        #endregion

        #region Команды

        /// <summary>
        /// Команда для перемещения на главную страницу 
        /// </summary>
        public ICommand GoToMainPageCommand { get; set; }

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

        // Обработчик добавления элемента схемы в Canvas
        private void ElementAddedHandler (IElement element)
        {
            
        }

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

            var schemeElements = elements.Where(x => x.ElementType == ElementType.SchemeElement).ToList();

            var tags = new List<object>();

            var wires = new List<Wire>();

            foreach (var element in elements)
            {
                tags.Add(element);

                //element.SetSelected(true);

                if (element.GetTag() is Wire wire)
                {
                    wires.Add(wire);
                    //element.SetSelected(true);
                }
            }


            //var t = wires.Where(x => x.Start is PinThumb | x.End is PinThumb).Select(x => x.Start).Select(x => x.GetParent()).ToList();
        }

        /// <summary>
        /// Метод для установки страницы
        /// </summary>
        /// <param name="page"></param>
        public void SetPage(Page page)
        {
            _Page = (SchemeEditorPage)page;
            _CurrentCanvas = _Page.Editor.Context.CurrentCanvas;
        }

        #endregion
    }
}
