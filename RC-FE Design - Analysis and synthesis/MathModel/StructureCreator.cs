using FractalElementDesigner.Controls;
using FractalElementDesigner.FEEditing;
using FractalElementDesigner.FEEditing.Controls;
using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.Cells;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Класс реализующий создание конструкции элемента
    /// </summary>
    class StructureCreator
    {
        /// <summary>
        /// Событие выполнения части работы
        /// </summary>
        public static event Action<double> OnDoWork;

        /// <summary>
        /// Событие изменения статуса выполнения процесса
        /// </summary>
        public static event Action<string> OnStateChange;

        // Метод для создания конструкции элемента
        public static RCStructure Create(FElementScheme scheme, RCStructure _structure)
        {
            RCStructureBase structure = new RCStructureBase();

            OnStateChange("Создание конструкции");

            // для отладки
            // для отладки
            // для отладки
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(5);
                OnDoWork(i + 1);
            }
            // для отладки
            // для отладки
            // для отладки

            OnStateChange("");

            var t = InitializeStructure(_structure);

            

            return t;
        }

        public static void InsertVisual(RCStructure structure, FESchemeModel schemeModel, FEControl structureEditorControl) 
        {
            // вставить слои
            foreach (var layer in structure.StructureLayers)
            {
                var editor = new Editor() { Context = new Context() };
                editor.Context.CurrentCanvas = structureEditorControl.CreateFECanvas();

                layer.Editor = editor;

                FitStructureToScheme(structure, schemeModel, layer.CellsType);

                Insert.StructureLayer(editor.Context.CurrentCanvas as FECanvas, layer);
                //Insert.StructureLayer(editor.Context.CurrentCanvas as FECanvas, layer, layer.CellsType);
            }
        }

        private static void FitStructureToScheme(RCStructure structure, FESchemeModel schemeModel, CellType cellType) 
        {


            CellType DefineCellType(int i, int j, int rowCount, CellType _layerType)
            {
                // первая строка
                if (i == 0)
                {
                    if (j != 0 | j != rowCount - 1)
                    {
                        return CellType.PlaceForContact;
                    }
                }
                // последняя строка
                if (i == rowCount - 1)
                {
                    if (j != 0 | j != rowCount - 1)
                    {
                        return CellType.PlaceForContact;
                    }
                }
                // первая колонка
                if (j == 0)
                {
                    // установить угловые ячейки как неактивные
                    if (i == 0 | i == rowCount - 1)
                    {
                        return CellType.None;
                    }
                    else
                    {
                        return CellType.PlaceForContact;
                    }
                }
                // последняя колонка
                if (j == rowCount - 1)
                {
                    // установить угловые ячейки как неактивные
                    if (i == 0 | i == rowCount - 1)
                    {
                        return CellType.None;
                    }
                    else
                    {
                        return CellType.PlaceForContact;
                    }
                }

                return _layerType;
            }
        }

        // Метод для инициализации структуры
        public static RCStructure InitializeStructure(RCStructure structure)
        {
            // извлечь число ячеек по горизонтали структуры
            structure.StructureProperties.TryGetValue("HorizontalCellsCount", out var horizontalStructureDimension);
            var horizontalStructureDimensionValue = (int)horizontalStructureDimension.Value + 2;// +2 добавляется для учёта контактных площадок
                                                                                                // извлечь число ячеек по вертикали структуры
            structure.StructureProperties.TryGetValue("VerticalCellsCount", out var verticalStructureDimension);
            var verticalStructureDimensionValue = (int)verticalStructureDimension.Value + 2;// +2 добавляется для учёта контактных площадок
            // новая структура
            var newStructure = structure;

            foreach (var layer in newStructure.StructureLayers)
            {
                for (int r = 0; r < verticalStructureDimensionValue; r++)
                {
                    var row = new ObservableCollection<StructureCellBase>();

                    for (int c = 0; c < horizontalStructureDimensionValue; c++)
                    {
                        row.Add(new StructureCellBase());
                    }

                    layer.StructureCells.Add(row);
                }
            }

            return newStructure;
        }
    }
}
