using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
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
    /// Главная ViewModel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="resolver">Ссылка на объект для определения ViewModel'ов</param>
        public MainViewModel(IViewModelsResolver resolver)
        {
            _resolver = resolver;

            // создать страницы
            _SynthesisPage = new SynthesisPage();
            _AnalysisPage = new AnalysisPage();
            _SchemeEditorPage = new SchemeEditorPage();
        
            _SchemeEditorPageViewModel = _resolver.GetViewModelInstance(SchemeEditorPageViewModelAlias);
            _SynthesisPageViewModel = _resolver.GetViewModelInstance(SynthesisPageViewModelAlias);
            _AnalysisPageViewModel = _resolver.GetViewModelInstance(AnalysisPageViewModelAlias);

            _SynthesisPageViewModel.SetPage(_SynthesisPage);
            _AnalysisPageViewModel.SetPage(_AnalysisPage);

            // Инициализировать команды
            InitializeCommands();
            
            _SynthesisPageViewModel.GoToMainPageCommand = GoToMainPageCommand;
            _AnalysisPageViewModel.GoToMainPageCommand = GoToMainPageCommand;

            // Перейти на главную страницу
            GoToMainPageCommandExecute(); 
            GoToAnalysisPageCommandExecute();// ДЛЯ ОТЛАДКИ
        }

        #region Константы

        public static readonly string SchemeEditorPageViewModelAlias = "SchemeEditorPageVM";
        public static readonly string SynthesisPageViewModelAlias = "SynthesisPageVM";
        public static readonly string AnalysisPageViewModelAlias = "AnalysisPageVM";

        #endregion

        #region Глобальные переменные

        /// <summary>
        /// Ссылка на объект для разрешения ViewModel'ов
        /// </summary>
        private readonly IViewModelsResolver _resolver;

        /// <summary>
        /// Ссылка на ViewModel страницы редактора схем
        /// </summary>
        private readonly IPageViewModel _SchemeEditorPageViewModel;
        /// <summary>
        /// Ссылка на ViewModel страницы синтеза
        /// </summary>
        private readonly IPageViewModel _SynthesisPageViewModel;
        /// <summary>
        /// Ссылка на ViewModel страницы анализа
        /// </summary>
        private readonly IPageViewModel _AnalysisPageViewModel;

        /// <summary>
        /// Страница синтеза
        /// </summary>
        private readonly Page _SynthesisPage;
        /// <summary>
        /// Страница анализа
        /// </summary>
        private readonly Page _AnalysisPage;
        /// <summary>
        /// Страница редактора схем
        /// </summary>
        private readonly Page _SchemeEditorPage;

        #endregion

        #region Свойства



        #endregion

        #region Команды

        private ICommand _goToSchemeEditorPageCommand;
        /// <summary>
        /// Команда для перемещения на страницу редактора схем
        /// </summary>
        public ICommand GoToSchemeEditorPageCommand
        {
            get
            {
                return _goToSchemeEditorPageCommand;
            }
            set
            {
                _goToSchemeEditorPageCommand = value;
                RaisePropertyChanged(nameof(GoToSchemeEditorPageCommand));
            }
        }

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

        private ICommand _goToSynthesisPageCommand;
        /// <summary>
        /// Команда для перемещения на страницу синтеза
        /// </summary>
        public ICommand GoToSynthesisPageCommand
        {
            get { return _goToSynthesisPageCommand; }
            set
            {
                _goToSynthesisPageCommand = value;
                RaisePropertyChanged(nameof(GoToSynthesisPageCommand));
            }
        }

        private ICommand _goToAnalysisPageCommand;
        /// <summary>
        /// Команда для перемещения на страницу анализа
        /// </summary>
        public ICommand GoToAnalysisPageCommand
        {
            get { return _goToAnalysisPageCommand; }
            set
            {
                _goToAnalysisPageCommand = value;
                RaisePropertyChanged(nameof(GoToAnalysisPageCommand));
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            GoToSchemeEditorPageCommand = new RelayCommand(GoToSchemeEditorPageCommandExecute);
            GoToMainPageCommand = new RelayCommand(GoToMainPageCommandExecute);
            GoToSynthesisPageCommand = new RelayCommand(GoToSynthesisPageCommandExecute);
            GoToAnalysisPageCommand = new RelayCommand(GoToAnalysisPageCommandExecute);
        }

        /// <summary>
        /// Метод для перемещения на страницу редактора схем
        /// </summary>
        private void GoToSchemeEditorPageCommandExecute()
        {
            Navigation.Navigation.Navigate(_SchemeEditorPage, _SchemeEditorPageViewModel);
        }

        /// <summary>
        /// Метод для перемещения на главную страницу 
        /// </summary>
        private void GoToMainPageCommandExecute()
        {
            Navigation.Navigation.Navigate(Navigation.Navigation.MainPageAlias, this);
        }

        /// <summary>
        /// Метод для перемещения на страницу синтеза 
        /// </summary>
        private void GoToSynthesisPageCommandExecute()
        {
            Navigation.Navigation.Navigate(_SynthesisPage, _SynthesisPageViewModel);
        }

        /// <summary>
        /// Метод для перемещения на страницу анализа
        /// </summary>
        private void GoToAnalysisPageCommandExecute()
        {
            Navigation.Navigation.Navigate(_AnalysisPage, _AnalysisPageViewModel);
        }

        #endregion
    }
}
