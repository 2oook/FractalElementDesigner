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
        public MainViewModel(IViewModelsResolver resolver)
        {
            _resolver = resolver;

            _MainPageViewModel = _resolver.GetViewModelInstance(MainPageViewModelAlias);

            InitializeCommands();
        }

        #region Constants

        public static readonly string MainPageViewModelAlias = "MainPageVM";
        public static readonly string NotFoundPageViewModelAlias = "404VM";

        #endregion

        #region Глобальные переменные

        private readonly IViewModelsResolver _resolver;

        private readonly INotifyPropertyChanged _MainPageViewModel;

        #endregion

        #region Свойства

        public INotifyPropertyChanged MainPageViewModel
        {
            get { return _MainPageViewModel; }
        }

        #endregion

        #region Команды

        private ICommand _goToPathCommand;
        public ICommand GoToPathCommand
        {
            get { return _goToPathCommand; }
            set
            {
                _goToPathCommand = value;
                RaisePropertyChanged("GoToPathCommand");
            }
        }

        private ICommand _goToPage1Command;
        public ICommand GoToPage1Command
        {
            get
            {
                return _goToPage1Command;
            }
            set
            {
                _goToPage1Command = value;
                RaisePropertyChanged("GoToPage1Command");
            }
        }

        private ICommand _goToPage2Command;
        public ICommand GoToPage2Command
        {
            get { return _goToPage2Command; }
            set
            {
                _goToPage2Command = value;
                RaisePropertyChanged("GoToPage2Command");
            }
        }

        private ICommand _goToPage3Command;
        public ICommand GoToPage3Command
        {
            get { return _goToPage3Command; }
            set
            {
                _goToPage3Command = value;
                RaisePropertyChanged("GoToPage3Command");
            }
        }

        #endregion

        #region Методы

        private void InitializeCommands()
        {

            GoToPathCommand = new RelayCommand<string>(GoToPathCommandExecute);
            GoToPage1Command = new RelayCommand<INotifyPropertyChanged>(GoToPage1CommandExecute);
            GoToPage2Command = new RelayCommand<INotifyPropertyChanged>(GoToPage2CommandExecute);
            GoToPage3Command = new RelayCommand<INotifyPropertyChanged>(GoToPage3CommandExecute);
        }

        private void GoToPathCommandExecute(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return;
            }

            Navigation.Navigation.Navigate(alias);
        }

        private void GoToPage1CommandExecute(INotifyPropertyChanged viewModel)
        {
            Navigation.Navigation.Navigate(Navigation.Navigation.MainPageAlias, this);
        }

        private void GoToPage2CommandExecute(INotifyPropertyChanged viewModel)
        {
            Navigation.Navigation.Navigate(Navigation.Navigation.Page2Alias, this);
        }

        private void GoToPage3CommandExecute(INotifyPropertyChanged viewModel)
        {
            Navigation.Navigation.Navigate(Navigation.Navigation.Page3Alias, this);
        }

        #endregion
    }
}
