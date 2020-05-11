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

        #endregion

        #region Команды

        /// <summary>
        /// Команда для перемещения на главную страницу 
        /// </summary>
        public ICommand GoToMainPageCommand { get; set; }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            
        }

        #endregion
    }
}
