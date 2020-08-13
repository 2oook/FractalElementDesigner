﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using FractalElementDesigner.StructureSchemeSynthesis;
using FractalElementDesigner.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;

namespace FractalElementDesigner.ViewModels
{
    /// <summary>
    /// ViewModel окна ввода параметров синтеза
    /// </summary>
    public class StructureSchemeSynthesisParametersWindowViewModel : INotifyPropertyChanged
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
        /// <summary>
        /// Объект параметров синтеза
        /// </summary>
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
                // если нет ошибок принимаем ввод пользователя
                if (StructureSchemeSynthesisParametersInstance.Error == null)
                {
                    _StructureSchemeSynthesisParametersWindow.AcceptUserInput();
                }
                else
                {
                    _StructureSchemeSynthesisParametersWindow.ShowValidationError();
                }
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

        /// <summary>
        /// Событие изменения свойства
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
