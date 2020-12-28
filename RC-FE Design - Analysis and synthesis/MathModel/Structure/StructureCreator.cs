using FractalElementDesigner.Controls;
using FractalElementDesigner.FEEditing;
using FractalElementDesigner.FEEditing.Controls;
using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.MathModel.Structure;
using FractalElementDesigner.RCWorkbenchLibrary;
using FractalElementDesigner.RCWorkbenchLibrary.Helpers;
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
        public static RCStructure Create(RCStructure structure)
        {
            OnStateChange("Создание конструкции");
            OnDoWork(0);

            var made_structure = InitializeStructure(structure);

            OnDoWork(100);
            OnStateChange("");

            return made_structure;
        }

        public static RCStructure CreateStructureByScheme(FElementScheme scheme, RCStructure structure)
        {
            OnStateChange("Создание конструкции");
            OnDoWork(0);

            FitLayersToScheme(structure, scheme.Model);

            OnDoWork(50);

            var made_structure = InitializeStructureByScheme(scheme, structure);

            OnDoWork(100);
            OnStateChange("");

            return made_structure;
        }

        // Метод для определения типов СЕГМЕНТОВ по типам ячеек в слоях (для ячеек структуры без КП)
        public static void ResolveSegmentsTypes(RCStructure structure) 
        {
            foreach (var row in structure.Segments)
            {
                foreach (var mainCell in row)
                {
                    CellType upperLayerCellType = CellType.None;
                    CellType lowerLayerCellType = CellType.None;

                    foreach (var layer in structure.StructureLayers)
                    {
                        // верхний слой
                        if (layer.Number == 0)
                        {
                            upperLayerCellType = mainCell.CellsInLayer[layer].CellType;
                        }
                        else if (layer.Number == 1)// нижний
                        {
                            lowerLayerCellType = mainCell.CellsInLayer[layer].CellType;
                        }
                    }

                    switch (upperLayerCellType)
                    {
                        case CellType.None:
                            {
                                switch (lowerLayerCellType)
                                {
                                    case CellType.None:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.EMPTY;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CellType.Cut:
                            {
                                switch (lowerLayerCellType)
                                {
                                    case CellType.Cut:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.EMPTY;
                                        }
                                        break;
                                    case CellType.R:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.Rn;
                                        }
                                        break;
                                    case CellType.NRk:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CellType.RC:
                            break;
                        case CellType.R:
                            {
                                switch (lowerLayerCellType)
                                {
                                    case CellType.Cut:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.Rv;
                                        }
                                        break;
                                    case CellType.R:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.R_C_NR;
                                        }
                                        break;
                                    case CellType.NRk:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.R_C_NRk;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CellType.Rk:
                            {
                                switch (lowerLayerCellType)
                                {
                                    case CellType.Cut:
                                        break;
                                    case CellType.R:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.Rk_C_NR;
                                        }
                                        break;
                                    case CellType.NRk:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.Rk_C_NRk;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            mainCell.SegmentType = StructureSegmentTypeEnum.EMPTY;
                            break;
                    }

                    switch (lowerLayerCellType)
                    {
                        case CellType.None:
                            {
                                switch (upperLayerCellType)
                                {
                                    case CellType.None:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.EMPTY;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CellType.Cut:
                            {
                                switch (upperLayerCellType)
                                {
                                    case CellType.Cut:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.EMPTY;
                                        }
                                        break;
                                    case CellType.RC:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.Rv;
                                        }
                                        break;
                                    case CellType.NRk:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CellType.R:
                            {
                                switch (upperLayerCellType)
                                {
                                    case CellType.Cut:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.Rn;
                                        }
                                        break;
                                    case CellType.RC:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.R_C_NR;
                                        }
                                        break;
                                    case CellType.Rk:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.Rk_C_NR;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CellType.NRk:
                            {
                                switch (upperLayerCellType)
                                {
                                    case CellType.Cut:
                                        break;
                                    case CellType.RC:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.R_C_NRk;
                                        }
                                        break;
                                    case CellType.Rk:
                                        {
                                            mainCell.SegmentType = StructureSegmentTypeEnum.Rk_C_NRk;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            mainCell.SegmentType = StructureSegmentTypeEnum.EMPTY;
                            break;
                    }
                }
            }
        }

        // Метод для вставки слоя
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

        // Метод для нумерования контактных площадок
        public static void NumerateContactPlatesByScheme(RCStructure structure) 
        {
            var in_pins = structure.Scheme.Model.OuterPins.Select((x, i) => new { index = i, pin = x }).Where(x => x.pin.State == OuterPinState.In).Select(x => x.index).ToList();
            var grounded_pins = structure.Scheme.Model.OuterPins.Select((x, i) => new { index = i, pin = x }).Where(x => x.pin.State == OuterPinState.Gnd).Select(x => x.index).ToList();
            var connected_pins = structure.Scheme.Model.OuterPins.Select((x, i) => new { index = i, pin = x }).Where(x => x.pin.State == OuterPinState.Con).Select(x => x.index).ToList();
            var float_pins = structure.Scheme.Model.OuterPins.Select((x, i) => new { index = i, pin = x }).Where(x => x.pin.State == OuterPinState.Float).Select(x => x.index).ToList();

            // первым нумеруется in затем остальные выводы
            foreach (var pinNumber in in_pins)
            {
                var pinNumberOnX = ConvertPinNumberToXPositionInStructure(pinNumber);
                var layerNumber = GetLayerByPinNunber(pinNumber);

                RCWorkbenchLibraryEntry.SetElementTypeToStructureCell(layerNumber, pinNumberOnX, 0, CellTypeToRCWorkbenchConverter.Convert(CellType.None));
            }

            foreach (var pinNumber in grounded_pins)
            {
                var pinNumberOnX = ConvertPinNumberToXPositionInStructure(pinNumber);
                var layerNumber = GetLayerByPinNunber(pinNumber);

                RCWorkbenchLibraryEntry.SetElementTypeToStructureCell(layerNumber, pinNumberOnX, 0, CellTypeToRCWorkbenchConverter.Convert(CellType.None));
            }

            foreach (var pinNumber in connected_pins)
            {
                var pinNumberOnX = ConvertPinNumberToXPositionInStructure(pinNumber);
                var layerNumber = GetLayerByPinNunber(pinNumber);

                RCWorkbenchLibraryEntry.SetElementTypeToStructureCell(layerNumber, pinNumberOnX, 0, CellTypeToRCWorkbenchConverter.Convert(CellType.None));
            }

            foreach (var pinNumber in float_pins)
            {
                var pinNumberOnX = ConvertPinNumberToXPositionInStructure(pinNumber);
                var layerNumber = GetLayerByPinNunber(pinNumber);

                RCWorkbenchLibraryEntry.SetElementTypeToStructureCell(layerNumber, pinNumberOnX, 0, CellTypeToRCWorkbenchConverter.Convert(CellType.None));
            }

            int ConvertPinNumberToXPositionInStructure(int _pinNumber) 
            {
                int result = -2;

                switch (_pinNumber)
                {
                    case 0: // сверху-слева
                        result = -1;
                        break;
                    case 1: // сверху-справа
                        result = structure.Segments.First().Count;
                        break;

                    case 2: // снизу-справа
                        result = structure.Segments.First().Count;
                        break;
                    case 3: // снизу-слева
                        result = -1;
                        break;
                    default:
                        break;
                }

                return result;
            }

            int GetLayerByPinNunber(int _pinNumber) 
            {
                if (_pinNumber == 0 | _pinNumber == 1)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }

        // Метод для наложения полученной схемы на слои структуры
        private static void FitLayersToScheme(RCStructure structure, FESchemeModel schemeModel) 
        {
            var upperLayer = structure.StructureLayers.First();
            var lowerLayer = structure.StructureLayers.Last();

            int rowCount = 5;//12;

            AddColumnToStructure(upperLayer, rowCount, CellType.Contact, CellType.None, CellType.None);
            AddColumnToStructure(lowerLayer, rowCount, CellType.Contact, CellType.None, CellType.None);

            AddColumnToStructure(upperLayer, rowCount, CellType.Rk, CellType.PlaceForContact, CellType.PlaceForContact);
            AddColumnToStructure(lowerLayer, rowCount, CellType.NRk, CellType.PlaceForContact, CellType.PlaceForContact);

            for (int c = 0; c < schemeModel.InnerConnections.Count; c++)
            {
                var columns = FElementScheme.AllowablePinsConnectionsOnLayer[(schemeModel.InnerConnections[c].ConnectionType, schemeModel.InnerConnections[c].PEType)];

                for (int i = 0; i < 2/*5*/; i++)
                {
                    AddColumnToStructure(upperLayer, rowCount, CellType.RC, CellType.PlaceForContact, CellType.PlaceForContact);
                    AddColumnToStructure(lowerLayer, rowCount, CellType.R, CellType.PlaceForContact, CellType.PlaceForContact);
                }

                foreach (var column in columns)
                {
                    AddColumnToStructure(upperLayer, rowCount, column.cellsTypes[0], CellType.PlaceForContact, CellType.PlaceForContact);
                    AddColumnToStructure(lowerLayer, rowCount, column.cellsTypes[1], CellType.PlaceForContact, CellType.PlaceForContact);
                }
            }

            for (int i = 0; i < 2/*5*/; i++)
            {
                AddColumnToStructure(upperLayer, rowCount, CellType.RC, CellType.PlaceForContact, CellType.PlaceForContact);
                AddColumnToStructure(lowerLayer, rowCount, CellType.R, CellType.PlaceForContact, CellType.PlaceForContact);
            }

            AddColumnToStructure(upperLayer, rowCount, CellType.Rk, CellType.PlaceForContact, CellType.PlaceForContact);
            AddColumnToStructure(lowerLayer, rowCount, CellType.NRk, CellType.PlaceForContact, CellType.PlaceForContact);

            AddColumnToStructure(upperLayer, rowCount, CellType.Contact, CellType.None, CellType.None);
            AddColumnToStructure(lowerLayer, rowCount, CellType.Contact, CellType.None, CellType.None);
        }

        private static void AddColumnToStructure(Layer layer, int rowsCount, CellType mainCellType, CellType upperCellType, CellType lowerCellType) 
        {
            // rowsCount включает ячейки для контактных площадок

            if (layer.Cells.Count == 0)
            {
                // для всех строк структуры
                for (int i = 0; i < rowsCount; i++)
                {
                    layer.Cells.Add(new ObservableCollection<Cell>());
                }
            }

            // для всех строк структуры
            for (int i = 0; i < layer.Cells.Count; i++)
            {
                var row = layer.Cells;

                if (i == 0)
                {
                    row[i].Add(new Cell() { Layer = layer, CellType = upperCellType });
                }
                else if (i == layer.Cells.Count-1)
                {
                    row[i].Add(new Cell() { Layer = layer, CellType = lowerCellType });
                }
                else
                {
                    row[i].Add(new Cell() { Layer = layer, CellType = mainCellType });
                }
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

        // Метод для инициализации структуры по схеме
        public static RCStructure InitializeStructureByScheme(FElementScheme scheme, RCStructure structure)
        {
            structure.Scheme = scheme;

            var first_layer = structure.StructureLayers.First();

            structure.InitializeByScheme(first_layer.Cells.Count, first_layer.Cells.First().Count);

            return structure;
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
                        layer.Cells[r][c].CellType = DefineCellType(r, c, verticalStructureDimensionValue, horizontalStructureDimensionValue,
                            layer.Name == RCStructureLayerTypeConstants.NR ? CellType.R : CellType.RC);
                    }
                }
            }

            CellType DefineCellType(int i, int j, int rowCount, int columnCount, CellType _layerType)
            {
                // первая строка или последняя
                if (i == 0 || i == rowCount - 1)
                {
                    if (j != 0 && j != columnCount - 1)
                    {
                        return CellType.PlaceForContact;
                    }
                    else
                    {
                        return CellType.None;
                    }
                }
                // первая колонка или последняя
                if (j == 0 || j == columnCount - 1)
                {
                    // установить угловые ячейки как неактивные
                    if (i != 0 && i != rowCount - 1)
                    {
                        return CellType.PlaceForContact;
                    }
                    else
                    {
                        return CellType.None;
                    }
                }

                return _layerType;
            }
        }
    }
}
