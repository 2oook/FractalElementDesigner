using FractalElementDesigner.FEEditing.Model.Cells;
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
    public class Layer
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Тип ячеек слоя
        /// </summary>
        public CellType CellsType { get; set; } = CellType.None;

        /// <summary>
        /// Матрица ячеек слоя
        /// </summary>
        public ObservableCollection<ObservableCollection<StructureCellBase>> StructureCells { get; set; } = new ObservableCollection<ObservableCollection<StructureCellBase>>();

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
