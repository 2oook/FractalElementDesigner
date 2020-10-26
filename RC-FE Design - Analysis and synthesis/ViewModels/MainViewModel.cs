using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using FractalElementDesigner.Commands;
using FractalElementDesigner.Navigating;
using FractalElementDesigner.Navigating.Interfaces;
using FractalElementDesigner.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FractalElementDesigner.ViewModels
{
    /// <summary>
    /// Главная ViewModel
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="resolver">Ссылка на объект для определения ViewModel'ов</param>
        public MainViewModel(IViewModelsResolver resolver)
        {
            _resolver = resolver;

            // создать страницы
            _StructureDesigningPage = new StructureDesigningPage();
        
            _StructureDesigningPageViewModel = _resolver.GetViewModelInstance(StructureDesigningPageViewModelAlias);

            // методы используются так как нет доступа к конструкторам viewmodel'oв
            _StructureDesigningPageViewModel.SetPage(_StructureDesigningPage);

            // Инициализировать команды
            InitializeCommands();

            // установить команду возвращения на главную страницу
            _StructureDesigningPageViewModel.GoToMainPageCommand = GoToMainPageCommand;

            // Зарегистрировать статические команды
            CommandManager.RegisterClassCommandBinding(typeof(Page), new CommandBinding(StaticCommandContainer.GoToStructureDesigningPageCommand, GoToStructureDesigningPageCommandExecute));

            // Перейти на главную страницу
            GoToMainPageCommandExecute();

            // Перейти на страницу проектирования // ОТЛАДКА
            GoToStructureDesigningPageCommandExecute(null, null);

            //GoToSchemeEditorPageCommandExecute(null, null); 
        }

        #region Константы

        /// <summary>
        /// Название-константа страницы  проектирования
        /// </summary>
        public static readonly string StructureDesigningPageViewModelAlias = "StructureDesigningPageVM";

        #endregion

        #region Глобальные переменные

        /// <summary>
        /// Ссылка на объект для разрешения ViewModel'ов
        /// </summary>
        private readonly IViewModelsResolver _resolver;

        /// <summary>
        /// Ссылка на ViewModel страницы проектирования
        /// </summary>
        private readonly IPageViewModel _StructureDesigningPageViewModel;

        /// <summary>
        /// Страница проектирования
        /// </summary>
        private readonly Page _StructureDesigningPage;

        #endregion

        #region Свойства



        #endregion

        #region Команды

        private ICommand _goToMainPageCommand;
        /// <summary>
        /// Команда для перемещения на главную страницу 
        /// </summary>
        public ICommand GoToMainPageCommand
        {
            get
            {
                return _goToMainPageCommand;
            }
            set
            {
                _goToMainPageCommand = value;
                RaisePropertyChanged(nameof(GoToMainPageCommand));
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            GoToMainPageCommand = new RelayCommand(GoToMainPageCommandExecute);
        }

        /// <summary>
        /// Метод для перемещения на главную страницу
        /// </summary>
        private void GoToMainPageCommandExecute()
        {
            Navigation.Navigate(Navigation.MainPageAlias, this);
        }

        /// <summary>
        /// Метод для перемещения на страницу проектирования
        /// </summary>
        private void GoToStructureDesigningPageCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Navigation.Navigate(_StructureDesigningPage, _StructureDesigningPageViewModel);
        }

        #endregion

        /// <summary>
        /// Ссылка на обработчик события изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для поднятия события изменения свойства
        /// </summary>
        /// <param name="propName">Имя свойства</param>
        protected virtual void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
