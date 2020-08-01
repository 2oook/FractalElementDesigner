using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RC_FE_Design___Analysis_and_synthesis.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Класс реализующий синтез схемы включения элемента
    /// </summary>
    class SchemeSynthesizer
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
        public static bool Synthesize(GeneticAlgorithm ga, StructureSchemeSynthesisParameters synthesisParameters, FElementScheme scheme) 
        {
            // тест!!!!!!!
            PhaseResponseCalculator.CalculatePhase(scheme, 10);
            // тест!!!!!!!

            bool result = true;

            OnStateChange("Выполнение синтеза");

            // рассчитать ФЧХ для схемы
            scheme.PhaseResponsePoints = InnerSchemePhaseResponseCalculator.CalculatePhaseResponseInScheme(
                synthesisParameters.MinFrequency, synthesisParameters.MaxFrequency, synthesisParameters.PointsCountAtFrequencyAxle, scheme);




            int n = scheme.FESections.Count;

            for (int i = 0; i < n; i++)
            {

            }

            // для отладки
            // для отладки
            // для отладки
            for (int i = 0; i < 100; i++)
            {                
                Thread.Sleep(5);
                OnDoWork(i+1);
            }
            // для отладки
            // для отладки
            // для отладки

            OnStateChange("");

            return result;
        }
    }
}
