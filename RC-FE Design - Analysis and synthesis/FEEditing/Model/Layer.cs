using FractalElementDesigner.FEEditing.Model.StructureElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model
{
    /// <summary>
    /// Слой структуры
    /// </summary>
    [Serializable]
    class Layer
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Матрица ячеек слоя
        /// </summary>
        public ObservableCollection<ObservableCollection<Cell>> Cells { get; set; } 

        private Editor editor;
        /// <summary>
        /// Редактор
        /// </summary>
        public Editor Editor
        {
            get { return editor; }
            set { editor = value; }
        }
    }
}
