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
        public static void CreateStructure(FElementScheme scheme, RCStructureBase structure) 
        {
            // инициализировать библиотеку
            RCWorkbenchLibraryEntry.InitiateLibrary();

            // симулировать pAnalyseParameters // СХЕМА №
            RCWorkbenchLibraryEntry.CreateCAnalyseParameters(5, 3, 0.98, 0.1, false, 100);

            // установить диапазон частот
            RCWorkbenchLibraryEntry.SetFrequencyRange(0.0, 100.0, 100);

            // создать структуру
            RCWorkbenchLibraryEntry.CreateRCGNRStructure(1.0, 1.0, 10, 10, 1.0, 0.001, 100, 0.218);
        }
    }
}
