using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using RC_FE_Design___Analysis_and_synthesis.StructureSchemeSynthesis;
using RC_FE_Design___Analysis_and_synthesis.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    public class StructureSchemeSynthesisParametersWindowViewModel : ViewModelBase
    {
        public StructureSchemeSynthesisParametersWindowViewModel(StructureSchemeSynthesisParametersWindow structureSchemeSynthesisParametersWindow)
        {
            _StructureSchemeSynthesisParametersWindow = structureSchemeSynthesisParametersWindow;

            InitializeCommands();
        }

        /// <summary>
        /// Ссылка на окно
        /// </summary>
        private readonly StructureSchemeSynthesisParametersWindow _StructureSchemeSynthesisParametersWindow;

        #region Свойства

        private StructureSchemeSynthesisParameters structureSchemeSynthesisParametersInstance = new StructureSchemeSynthesisParameters();

        public StructureSchemeSynthesisParameters StructureSchemeSynthesisParametersInstance
        {
            get { return structureSchemeSynthesisParametersInstance; }
            set 
            { 
                structureSchemeSynthesisParametersInstance = value;
                RaisePropertyChanged(nameof(StructureSchemeSynthesisParametersInstance));
            }
        }


        #endregion

        #region Команды

        private ICommand okCommand;

        /// <summary>
        /// Команда для создания новой структуры
        /// </summary>
        public ICommand OkCommand
        {
            get
            {
                return okCommand;
            }
            set
            {
                okCommand = value;
                RaisePropertyChanged(nameof(OkCommand));
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для обработки ОК
        /// </summary>
        private void OK_Handler()
        {
            try
            {
                _StructureSchemeSynthesisParametersWindow.AcceptUserInput();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeCommands()
        {
            OkCommand = new RelayCommand(OK_Handler);
        }

        #endregion
    }
}
