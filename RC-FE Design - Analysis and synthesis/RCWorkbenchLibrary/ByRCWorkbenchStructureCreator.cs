using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.FEEditing.Model.StructureElements;
using FractalElementDesigner.MathModel;
using FractalElementDesigner.RCWorkbenchLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.RCWorkbenchLibrary
{
    /// <summary>
    /// Класс для создания структуры на стороне библиотеки RCWorkbench
    /// </summary>
    class ByRCWorkbenchStructureCreator
    {
        public static void CreateStructure(FElementScheme scheme, RCStructure structure) 
        {
            // параметры секций задаются отдельно, но для RCWorkbench (в текущей реализации) нужно задавать из один раз на всю структуру
            // поэтому они обобщаются (берутся с первой секции)           

            // инициализировать библиотеку
            RCWorkbenchLibraryEntry.InitiateLibrary();

            // СХЕМА не важна // характеристика - ФЧХ входного импеданса
            RCWorkbenchLibraryEntry.CreateCAnalyseParameters(5, 3, 0.98, 0.1, false, scheme.SynthesisParameters.PointsCountAtFrequencyAxle);

            var maxFrequency = Math.Pow(10, scheme.SynthesisParameters.MaxFrequencyLn);
            var minFrequency = Math.Pow(10, scheme.SynthesisParameters.MinFrequencyLn);

            // установить диапазон частот
            RCWorkbenchLibraryEntry.SetFrequencyRange(minFrequency, maxFrequency, scheme.SynthesisParameters.PointsCountAtFrequencyAxle);

            // извлечь число ячеек по горизонтали структуры
            structure.StructureProperties.TryGetValue("HorizontalCellsCount", out var horizontalStructureDimension);
            var horizontalStructureDimensionValue = (int)horizontalStructureDimension.Value;
            // извлечь число ячеек по вертикали структуры
            structure.StructureProperties.TryGetValue("VerticalCellsCount", out var verticalStructureDimension);
            var verticalStructureDimensionValue = (int)verticalStructureDimension.Value;

            // создать структуру
            RCWorkbenchLibraryEntry.CreateRCGNRStructure(
                scheme.Model.FESections.First().SectionParameters.R, 
                scheme.Model.FESections.First().SectionParameters.C,
                horizontalStructureDimensionValue,
                verticalStructureDimensionValue, 
                1.0,
                scheme.Model.FESections.First().SectionParameters.G, 
                100,
                scheme.Model.FESections.First().SectionParameters.N);
        }

        public static void CreateStructureByScheme(FElementScheme scheme, RCStructure structure)
        {
            // параметры секций задаются отдельно, но для RCWorkbench (в текущей реализации) нужно задавать из один раз на всю структуру
            // поэтому они обобщаются (берутся с первой секции)           

            // инициализировать библиотеку
            RCWorkbenchLibraryEntry.InitiateLibrary();

            // СХЕМА не важна // характеристика - ФЧХ входного импеданса
            RCWorkbenchLibraryEntry.CreateCAnalyseParameters(5, 3, 0.98, 0.1, false, scheme.SynthesisParameters.PointsCountAtFrequencyAxle);

            var maxFrequency = Math.Pow(10, scheme.SynthesisParameters.MaxFrequencyLn);
            var minFrequency = Math.Pow(10, scheme.SynthesisParameters.MinFrequencyLn);

            // установить диапазон частот
            RCWorkbenchLibraryEntry.SetFrequencyRange(minFrequency, maxFrequency, scheme.SynthesisParameters.PointsCountAtFrequencyAxle);

            // создать структуру
            RCWorkbenchLibraryEntry.CreateRCGNRStructure(
                scheme.Model.FESections.First().SectionParameters.R,
                scheme.Model.FESections.First().SectionParameters.C,
                structure.Segments.First().Count - 2,
                structure.Segments.Count - 2,
                1.0,
                scheme.Model.FESections.First().SectionParameters.G,
                100,
                scheme.Model.FESections.First().SectionParameters.N);

            ApplyCellTypesToRCWorkbenchStructure(structure);
        }

        public static void CreateStructureStraightByScheme(FElementScheme scheme, RCStructure structure)
        {
            // создать структуру
            RCWorkbenchLibraryEntry.CreateRCGNRStructureStraight(
                scheme.Model.FESections.First().SectionParameters.R,
                scheme.Model.FESections.First().SectionParameters.C,
                structure.Segments.First().Count - 2,
                structure.Segments.Count - 2,
                1.0,
                scheme.Model.FESections.First().SectionParameters.G,
                100,
                scheme.Model.FESections.First().SectionParameters.N);

            ApplyCellTypesToRCWorkbenchStructure(structure);
        }

        // Метод для применения типов ЯЧЕЕК к структуре на стороне RCWorkbench
        private static void ApplyCellTypesToRCWorkbenchStructure(RCStructure structure) 
        {
            foreach (var layer in structure.StructureLayers)
            {
                foreach (var row in layer.Cells)
                {
                    foreach (var cell in row)
                    {
                        if (cell.CellType == CellType.Rk | cell.CellType == CellType.NRk)
                        {
                            //RCWorkbenchLibraryEntry.SetElementTypeDirectlyToStructureCell(layer.Number, cell.MainCell.Position.x - 1, cell.MainCell.Position.y - 1,
                                //CellTypeToRCWorkbenchConverter.Convert(cell.CellType));
                        }
                        else
                        {
                            RCWorkbenchLibraryEntry.SetElementTypeToStructureCell(layer.Number, cell.MainCell.Position.x - 1, cell.MainCell.Position.y - 1,
                                CellTypeToRCWorkbenchConverter.Convert(cell.CellType));
                        }
                    }
                } 
            }
        }
    }
}
