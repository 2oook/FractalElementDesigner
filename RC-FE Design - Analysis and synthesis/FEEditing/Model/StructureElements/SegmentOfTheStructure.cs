using FractalElementDesigner.MathModel.Structure;
using GalaSoft.MvvmLight;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model.StructureElements
{
    /// <summary>
    /// Сегмент конструкции ФРЭ
    /// </summary>
    [Serializable]
    class SegmentOfTheStructure 
    {
        public SegmentOfTheStructure(string number)
        {
            Number = number;
        }

        /// <summary>
        /// Номер сегмента
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Позиция сегмента в структуре
        /// </summary>
        public (int x, int y) Position;

        /// <summary>
        /// Тип сегмента
        /// </summary>
        public StructureSegmentTypeEnum SegmentType = StructureSegmentTypeEnum.R_C_NR;

        /// <summary>
        /// Словарь ячеек сегмента по слоям (1 ячейка на слой)
        /// </summary>
        public Dictionary<Layer, Cell> CellsInLayer { get; set; } = new Dictionary<Layer, Cell>();
    }
}
