using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    /// <summary>
    /// ViewModel страницы анализа
    /// </summary>
    public class AnalysisViewModel : ViewModelBase, IPageViewModel
    {
        public AnalysisViewModel()
        {
            InitializeCommands();
        }

        #region Глобальные переменные



        #endregion

        #region Свойства

        private Project _project;

        public Project Project
        {
            get { return _project; }
            set { _project = value; }
        }

        private Visibility canvasVisibility = Visibility.Collapsed;

        public Visibility CanvasVisibility
        {
            get 
            { 
                return canvasVisibility; 
            }
            set 
            { 
                canvasVisibility = value;
                RaisePropertyChanged(nameof(CanvasVisibility));
            }
        }

        #endregion

        #region Команды

        /// <summary>
        /// Команда для перемещения на главную страницу 
        /// </summary>
        public ICommand GoToMainPageCommand { get; set; }

        private ICommand newStructureCommand;
        /// <summary>
        /// Команда для создания новой структуры
        /// </summary>
        public ICommand NewStructureCommand
        {
            get
            {
                return newStructureCommand;
            }
            set
            {
                newStructureCommand = value;
                RaisePropertyChanged(nameof(NewStructureCommand));
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            NewStructureCommand = new RelayCommand(CreateNewStructure);
        }

        private void CreateNewStructure() 
        {
            // создать проект


            // вывести окно параметров структуры


            // показать область проектирования
            CanvasVisibility = Visibility.Visible;
        }

        #endregion
    }
}
