using GalaSoft.MvvmLight;
using FractalElementDesigner.ProjectTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using FractalElementDesigner.FEEditing.Model.Cells;
using FractalElementDesigner.MathModel.Structure;
using MathNet.Numerics.LinearAlgebra;
using System.Numerics;

namespace FractalElementDesigner.FEEditing.Model
{
    /// <summary>
    /// Базовый класс структуры 
    /// </summary>
    class RCStructureBase : INotifyPropertyChanged, IProjectTreeItem
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
        /// Матрица ячеек
        /// </summary>
        public ObservableCollection<ObservableCollection<StructureCellBase>> Cells { get; set; } = new ObservableCollection<ObservableCollection<StructureCellBase>>();

        /// <summary>
        /// Словарь слоёв структуры
        /// </summary>
        public ObservableCollection<Layer> StructureLayers { get; set; } = new ObservableCollection<Layer>();

        /// <summary>
        /// Точки функции ФЧХ
        /// </summary>
        [NonSerialized]
        public List<(double frequency, double phase)> PhaseResponsePoints;

        public void Initialize(int verticalStructureDimensionValue, int horizontalStructureDimensionValue)
        {
            for (int r = 0; r < verticalStructureDimensionValue; r++)
            {
                var row = new ObservableCollection<StructureCellBase>();

                for (int c = 0; c < horizontalStructureDimensionValue; c++)
                {
                    var cell = new StructureCellBase(r.ToString() + c.ToString());

                    int pins_counter = 0;

                    foreach (var layer in StructureLayers)
                    {
                        var cell_in_layer = new CellInLayer();

                        cell_in_layer.Pins = new List<Pin>()
                        {
                            new Pin() { Number = 1 + pins_counter, CellInLayer = cell_in_layer },
                            new Pin() { Number = 2 + pins_counter, CellInLayer = cell_in_layer },
                            new Pin() { Number = 4 + pins_counter, CellInLayer = cell_in_layer },
                            new Pin() { Number = 3 + pins_counter, CellInLayer = cell_in_layer }
                        };

                        cell_in_layer.MainCell = cell;

                        cell.CellsInLayer.Add(layer, cell_in_layer);

                        pins_counter += cell_in_layer.Pins.Count;
                    }

                    cell.YParametersMatrix = Matrix<Complex>.Build.DenseOfArray(new Complex[pins_counter, pins_counter]);

                    row.Add(cell);
                }

                Cells.Add(row);
            }

            foreach (var layer in StructureLayers) 
            {
                var cells_in_layer = new ObservableCollection<ObservableCollection<CellInLayer>>(Cells.Select(x => new ObservableCollection<CellInLayer>(x.Select(y => y.CellsInLayer[layer]))));
                layer.Cells = cells_in_layer;
            }
        }


        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для поднятия события изменения свойства
        /// </summary>
        /// <param name="propName">Имя свойства</param>
        protected virtual void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
