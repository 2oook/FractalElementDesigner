using GalaSoft.MvvmLight;
using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    /// <summary>
    /// ViewModel страницы создания новой структуры
    /// </summary>
    public class NewStructureWindowViewModel : ViewModelBase
    {
        public NewStructureWindowViewModel()
        {
            // установить выбор структуры по умолчанию
            structureTypes.TryGetValue("R-CG-NR", out var selectedStructureType);
            SelectedStructureType = selectedStructureType;
        }

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
    }
}
