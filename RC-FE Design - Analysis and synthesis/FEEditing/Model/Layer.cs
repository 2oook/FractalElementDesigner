using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.ProjectTree;
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
    class Layer: IProjectTreeItem
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Номер
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Ссылка на структуру
        /// </summary>
        public RCStructure ParentStructure { get; set; }

        /// <summary>
        /// Матрица ячеек слоя
        /// </summary>
        public ObservableCollection<ObservableCollection<Cell>> Cells { get; set; } = new ObservableCollection<ObservableCollection<Cell>>();

        [NonSerialized]
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
