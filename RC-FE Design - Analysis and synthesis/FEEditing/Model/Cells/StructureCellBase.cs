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

namespace FractalElementDesigner.FEEditing.Model.Cells
{
    /// <summary>
    /// Базовый класс ячейки структуры
    /// </summary>
    class StructureCellBase 
    {
        public StructureCellBase(string number)
        {
            Number = number;
        }

        /// <summary>
        /// Номер ячейки
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Словарь выводов ячейки
        /// </summary>
        public Dictionary<Layer, CellInLayer> CellsInLayer { get; set; } = new Dictionary<Layer, CellInLayer>();

        /// <summary>
        /// Матрица проводимости ячейки
        /// </summary>
        public Matrix<Complex> YParametersMatrix { get; set; }
    }
}
