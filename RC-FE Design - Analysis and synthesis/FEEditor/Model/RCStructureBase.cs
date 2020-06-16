using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Model
{
    /// <summary>
    /// Базовый структуры 
    /// </summary>
    [Serializable]
    public class RCStructureBase : ViewModelBase
    {
        /// <summary>
        /// Порядковый номер структуры
        /// </summary>
        private static int StructureCurrentNumber { get; set; } = 0;

        /// <summary>
        /// Метод для получения следующего номера структуры
        /// </summary>
        /// <returns>Номер структуры</returns>
        protected static int GetNextStructureNumber() 
        {
            return StructureCurrentNumber++;
        }

        /// <summary>
        /// Название структуры
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Словарь слоёв структуры
        /// </summary>
        public ObservableCollection<Layer> StructureLayers { get; set; } = new ObservableCollection<Layer>();
    }
}
