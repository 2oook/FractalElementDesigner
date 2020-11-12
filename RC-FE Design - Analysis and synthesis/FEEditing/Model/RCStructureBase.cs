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
using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.MathModel.Structure;
using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using FractalElementDesigner.StructureSchemeSynthesis;

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
        /// Число выводов сегмента конструкции
        /// </summary>
        public int PinsCountOfSegment;

        /// <summary>
        /// Матрица сегментов
        /// </summary>
        public ObservableCollection<ObservableCollection<SegmentOfTheStructure>> Segments { get; set; } = new ObservableCollection<ObservableCollection<SegmentOfTheStructure>>();

        private ObservableCollection<Layer> structureLayers = new ObservableCollection<Layer>();
        /// <summary>
        /// Словарь слоёв структуры
        /// </summary>
        public ObservableCollection<Layer> StructureLayers 
        { 
            get => structureLayers; 
            set
            {
                structureLayers = value;
                RaisePropertyChanged(nameof(StructureLayers));
            } 
        }

        /// <summary>
        /// Параметры
        /// </summary>
        public StructureSchemeSynthesisParameters SynthesisParameters;

        /// <summary>
        /// Точки функции ФЧХ
        /// </summary>
        [NonSerialized]
        public List<(double frequency, double phase)> PhaseResponsePoints = new List<(double frequency, double phase)>();

        // Метод для инициализации структуры
        public void Initialize(int verticalStructureDimensionValue, int horizontalStructureDimensionValue)
        {
            // число выводов слоя сегмента
            var pins_count_in_layer = 4;
            // найти общее число выводов сегмента 
            PinsCountOfSegment = pins_count_in_layer * StructureLayers.Count;

            for (int r = 0; r < verticalStructureDimensionValue; r++)
            {
                var row = new ObservableCollection<SegmentOfTheStructure>();

                for (int c = 0; c < horizontalStructureDimensionValue; c++)
                {
                    var cell = new SegmentOfTheStructure(r.ToString() + c.ToString()) { Position = { x = c, y = r } };

                    int pins_counter = 0;

                    foreach (var layer in StructureLayers)
                    {
                        var cell_in_layer = new Cell() { Layer = layer };

                        // список выводов можно задать в виде параметра метода
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

                Segments.Add(row);
            }

            foreach (var layer in StructureLayers)
            {
                // найти ячейки слоя
                var cells_in_layer = new ObservableCollection<ObservableCollection<Cell>>(Segments.Select(x => new ObservableCollection<Cell>(x.Select(y => y.CellsInLayer[layer]))));
                layer.Cells = cells_in_layer;
            }
        }

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для поднятия события изменения свойства
        /// </summary>
        /// <param name="propName">Имя свойства</param>
        protected virtual void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
