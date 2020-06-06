using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using RC_FE_Design___Analysis_and_synthesis.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    /// <summary>
    /// ViewModel страницы создания новой структуры
    /// </summary>
    public class NewStructureWindowViewModel : ViewModelBase
    {
        public NewStructureWindowViewModel(NewStructureWindow newStructureWindow)
        {
            _newStructureWindow = newStructureWindow;

            // установить выбор структуры по умолчанию
            structureTypes.TryGetValue("R-CG-NR", out var selectedStructureType);
            SelectedStructureType = selectedStructureType;

            InitializeCommands();
        }

        /// <summary>
        /// Ссылка на окно
        /// </summary>
        private readonly NewStructureWindow _newStructureWindow;

        /// <summary>
        /// Экземпляр регулярного выражения соответствующее вещественному числу
        /// </summary>
        private static readonly Regex _realNumberRegex = new Regex("^[-+]?[0-9]*[.,]?[0-9]+$");

        #region Свойства

        /// <summary>
        /// Структуры для выбора и создания новоц структуры
        /// </summary>
        private Dictionary<string, RCStructure> StructuresForChoosing = new Dictionary<string, RCStructure>();

        private Dictionary<string, RCStructureTemplate> structureTypes = RCStructure.AllTemplateStructures;
        /// <summary>
        /// Все типы RC структур
        /// </summary>
        public Dictionary<string, RCStructureTemplate> StructureTypes
        {
            get
            {
                return structureTypes;
            }
            set
            {
                structureTypes = value;
                RaisePropertyChanged(nameof(StructureTypes));
            }
        }

        private RCStructureTemplate selectedStructureType = null;
        /// <summary>
        /// Выбранный тип структуры
        /// </summary>
        public RCStructureTemplate SelectedStructureType
        {
            get
            {
                return selectedStructureType;
            }
            set
            {
                // если структуры для выбора не содержат данной структуры создать её и установить в качестве текущей
                if (!StructuresForChoosing.ContainsKey(value.Name))
                {
                    var structure = new RCStructure(value.Name);
                    StructuresForChoosing.Add(value.Name, structure);
                    CurrentStructure = structure;
                }
                else
                {
                    StructuresForChoosing.TryGetValue(value.Name, out var structure);
                    CurrentStructure = structure;
                }

                selectedStructureType = value;
                RaisePropertyChanged(nameof(SelectedStructureType));
            }
        }

        private RCStructure currentStructure = null;
        /// <summary>
        /// Текущая структура
        /// </summary>
        public RCStructure CurrentStructure
        {
            get
            {
                return currentStructure;
            }
            set
            {
                currentStructure = value;
                RaisePropertyChanged(nameof(CurrentStructure));
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
            if (CurrentStructure == null) return;

            bool flag = false;

            // обойти все свойства структуры
            foreach (var property in CurrentStructure.StructureProperties)
            {
                if (!_realNumberRegex.IsMatch(property.Value.Value.ToString()))
                {
                    flag = true;
                    break;
                }
            }

            // если ввод некорректен
            if (flag)
            {
                _newStructureWindow.ShowValidationError();
            }
            else
            {
                _newStructureWindow.AcceptUserInput();
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
