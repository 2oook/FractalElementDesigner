using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditor.Model.Cells
{
    /// <summary>
    /// Базовый класс ячейки структуры
    /// </summary>
    public class StructureCellBase : ViewModelBase
    {
        /// <summary>
        /// Делегат для вызова метода применения инструмента
        /// </summary>
        public static Action<StructureCellBase> ApplyToolDelegate = delegate { };

        private CellType cellType = CellType.None;
        /// <summary>
        /// Тип ячейки структуры
        /// </summary>
        public CellType CellType 
        { 
            get => cellType;
            set 
            {
                cellType = value;
                RaisePropertyChanged(nameof(CellType));
            }
        }

        /// <summary>
        /// Метод для применения инструмента
        /// </summary>
        public void ApplyTool()
        {
            ApplyToolDelegate(this);
        }
    }
}
