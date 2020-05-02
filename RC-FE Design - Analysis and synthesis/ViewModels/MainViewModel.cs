using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            // Создать страницы 404
            _NotFoundPageViewModel = _resolver.GetViewModelInstance(NotFoundPageViewModelAlias);

            // Инициализировать команды
            InitializeCommands();

            // Перейти на главную страницу
            GoToMainPageCommandExecute();
        }

        #region Константы

        public static readonly string NotFoundPageViewModelAlias = "404VM";

        #endregion

        #region Глобальные переменные

        /// <summary>
        /// Ссылка на объект для разрешения ViewModel'ов
        /// </summary>
        private readonly IViewModelsResolver _resolver;

        /// <summary>
        /// Ссылка на ViewModel пустой страницы
        /// </summary>
        private readonly INotifyPropertyChanged _NotFoundPageViewModel;

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
            GoToMainPageCommand = new RelayCommand(GoToMainPageCommandExecute);
            GoToSynthesisPageCommand = new RelayCommand(GoToSynthesisPageCommandExecute);
            GoToAnalysisPageCommand = new RelayCommand(GoToAnalysisPageCommandExecute);
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
            Navigation.Navigation.Navigate(Navigation.Navigation.SynthesisPageAlias, this);
        }

        /// <summary>
        /// Метод для перемещения на страницу анализа
        /// </summary>
        private void GoToAnalysisPageCommandExecute()
        {
            Navigation.Navigation.Navigate(Navigation.Navigation.AnalysisPageAlias, this);
        }

        #endregion
    }
}
