using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Класс реализующий синтез конструкции элемента
    /// </summary>
    class StructureSynthesizer
    {
        /// <summary>
        /// Событие выполнения части работы
        /// </summary>
        public static event Action<double> OnDoWork;

        /// <summary>
        /// Событие изменения статуса выполнения процесса
        /// </summary>
        public static event Action<string> OnStateChange;

        // Метод для синтезирования схемы включения элемента
        public static void Synthesize(StructureSchemeSynthesisParameters synthesisParameters, RCStructureBase structure)
        {
            OnStateChange("Выполнение синтеза");

            //IGeneticAlgorithm ga = new SingleStageGeneticAlgorithm(synthesisParameters, 100, scheme);

            //SingleStageGeneticAlgorithm.OnDoWork += SingleStageGeneticAlgorithm_OnDoWork;

            //ga.Start();

            // для примера
            // рассчитать ФЧХ для конструкции
            structure.PhaseResponsePoints = PhaseResponseCalculatorForStructureForAllFrequencies.CalculatePhaseResponseInStructure(
                synthesisParameters.MinFrequencyLn, synthesisParameters.MaxFrequencyLn, synthesisParameters.PointsCountAtFrequencyAxle, structure);
            // для примера
            //var t = ga.GetPopulation();
            //t.Add(scheme);
            // для примера

            OnStateChange("");
        }

        private static void SingleStageGeneticAlgorithm_OnDoWork(double obj)
        {
            OnDoWork(obj);
        }
    }
}
