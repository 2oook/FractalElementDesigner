using FractalElementDesigner.Controls;
using FractalElementDesigner.FEEditing;
using FractalElementDesigner.FEEditing.Controls;
using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.MathModel.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Класс реализующий создание конструкции RC-G-NR элемента
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
        public static RCStructure Create(FElementScheme scheme, RCStructure structure)
        {
            FitLayerToScheme(structure, scheme.Model);

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

            var t = InitializeStructure(structure);

            

            return t;
        }

        public static void InsertVisual(RCStructure structure, FEControl structureEditorControl) 
        {
            // вставить слои
            foreach (var layer in structure.StructureLayers)
            {
                var editor = new Editor() { Context = new Context() };
                editor.Context.CurrentCanvas = structureEditorControl.CreateFECanvas();

                layer.Editor = editor;


                Insert.StructureLayer(editor.Context.CurrentCanvas as FECanvas, structure, layer);
            }
        }

        // Метод для наложения полученной схемы на слои структуры
        private static void FitLayerToScheme(RCStructure structure, FESchemeModel schemeModel) 
        {
            var schemeInLayerContext = ExpandConnectionsAndGroundsOnLayers(structure, schemeModel);

            // обойти слои структуры
            foreach (var layer in schemeInLayerContext.Keys)
            {
                var layerData = schemeInLayerContext[layer];
            }

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

        // Метод для получения соединений и заземлений в контексте слоёв структуры
        private static Dictionary<Layer, List<List<LayerConnectionItem>>> ExpandConnectionsAndGroundsOnLayers(RCStructure structure, FESchemeModel schemeModel) 
        {
            if (structure.StructureLayers.Count != schemeModel.InnerConnections[0].FirstSection.Pins.Count / 2)
            {
                throw new Exception("Количество слоёв не совпадает с количеством выводов БКЭ");
            }

            // структура слоёв с соединениями
            var layerInfo = new Dictionary<Layer, List<List<LayerConnectionItem>>>();

            // инициализация структуры
            foreach (var layer in structure.StructureLayers)
                layerInfo.Add(layer, new List<List<LayerConnectionItem>>());

            for (int c = 0; c < schemeModel.InnerConnections.Count; c++)
            {
                var localMatrix = FElementScheme.AllowablePinsConnections[schemeModel.InnerConnections[c].ConnectionType];
                var bound = localMatrix.ConnectionMatrix.GetUpperBound(0);

                // добавить 2 мерные представления пинов в структуру слоев
                foreach (var key in layerInfo.Keys)
                {
                    var lst = new List<LayerConnectionItem>();

                    layerInfo[key].Add(lst);

                    for (int i = 0; i < (bound + 1) / 2; i++)
                        lst.Add(new LayerConnectionItem() { Layer = key });
                }

                // обойти матрицу инцидентности
                for (int i = 0; i <= bound; i++)
                {
                    for (int j = i + 1; j <= bound; j++)
                    {
                        if (localMatrix.ConnectionMatrix[i, j] == 1)
                        {
                            var layerConnectionItem_i = MapIndexToLayerConnectionItem(i, c, layerInfo);
                            var layerConnectionItem_j = MapIndexToLayerConnectionItem(j, c, layerInfo);

                            // соединить представления пинов
                            layerConnectionItem_i.Connection.Add(layerConnectionItem_j);
                            layerConnectionItem_j.Connection.Add(layerConnectionItem_i);
                        }
                    }
                }

                // учесть заземления
                var pEVector = FElementScheme.AllowablePinsConnections[schemeModel.InnerConnections[c].ConnectionType].PEVector[schemeModel.InnerConnections[c].PEType];

                for (int i = 0; i < pEVector.Length; i++)
                {
                    if (pEVector[i] == 1)
                    {
                        var layerConnectionItem = MapIndexToLayerConnectionItem(i, c, layerInfo);
                        layerConnectionItem.Grounded = true;
                    }
                }
            }

            return layerInfo;

            // Метод для получения LayerConnectionItem по индексу из матрицы инцидентности
            LayerConnectionItem MapIndexToLayerConnectionItem(int index, int connNumber, Dictionary<Layer, List<List<LayerConnectionItem>>> layerDictionary)
            {
                var keys = layerDictionary.Keys.ToList();

                switch (index)
                {
                    case 0:
                        return layerDictionary[keys[0]][connNumber][0];
                    case 1:
                        return layerDictionary[keys[0]][connNumber][1];
                    case 2:
                        return layerDictionary[keys[1]][connNumber][1];
                    case 3:
                        return layerDictionary[keys[1]][connNumber][0];
                }

                return null;
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

            newStructure.Initialize(verticalStructureDimensionValue, horizontalStructureDimensionValue);

            InitializeCellsTypesOfTheStructure(structure, verticalStructureDimensionValue, horizontalStructureDimensionValue);

            return newStructure;
        }

        // Метод для инициализации ячеек в редакторе структуры
        private static void InitializeCellsTypesOfTheStructure(RCStructure structure, int verticalStructureDimensionValue, int horizontalStructureDimensionValue) 
        {
            // обойти слои структуры
            foreach (var layer in structure.StructureLayers)
            {
                for (int r = 0; r < verticalStructureDimensionValue; r++)
                {
                    for (int c = 0; c < horizontalStructureDimensionValue; c++)
                    {
                        layer.Cells[r][c].CellType = DefineCellType(r, c, verticalStructureDimensionValue, horizontalStructureDimensionValue, layer.Name == RCStructureLayerTypeConstants.NR ? CellType.R : CellType.RC);
                    }
                }
            }

            CellType DefineCellType(int i, int j, int rowCount, int columnCount, CellType _layerType)
            {
                // первая строка или последняя
                if (i == 0 || i == rowCount - 1)
                {
                    if (j != 0 | j != columnCount - 1)
                    {
                        return CellType.PlaceForContact;
                    }
                }
                // первая колонка или последняя
                if (j == 0 || j == columnCount - 1)
                {
                    // установить угловые ячейки как неактивные
                    if (i != 0 | i != rowCount - 1)
                    {
                        return CellType.PlaceForContact;
                    }
                }

                return _layerType;
            }
        }
    }
}
