using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.MathModel;
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

            // симулировать pAnalyseParameters // СХЕМА №
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
    }
}
