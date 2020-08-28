using FractalElementDesigner.FEEditing.Model.StructureElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.IO.ProjectSaveModel
{
    [Serializable]
    public class SavingLayer
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
        public ObservableCollection<ObservableCollection<SavingStructureCellBase>> StructureCells { get; set; } = new ObservableCollection<ObservableCollection<SavingStructureCellBase>>();
    }
}
