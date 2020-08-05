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
        public static List<FElementScheme> Synthesize(StructureSchemeSynthesisParameters synthesisParameters, FElementScheme scheme) 
        {
            OnStateChange("Выполнение синтеза");

            var ga = new GeneticAlgorithm(synthesisParameters, 100);

            ga.InitiatePopulation(scheme);

            // для сравнения мутировавших особей
            //var t = ga.Population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();

            ga.MutatePopulation();

            // для сравнения мутировавших особей
            //var t2 = ga.Population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();

            ga.CrossPopulation();

            ga.RatePopulation();

            // для просмотра оценки 
            var t4 = ga.Population.OrderByDescending(x => x.Model.Rate).Select(x => x.Model).ToList();


            //var t = population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();
            //var t1 = population.Select(x => x.Model.InnerConnections.Select(y => y.PEType).ToList()).ToList();

            // рассчитать ФЧХ для схемы
            scheme.Model.PhaseResponsePoints = InnerSchemePhaseResponseCalculator.CalculatePhaseResponseInScheme(
                synthesisParameters.MinFrequencyLn, synthesisParameters.MaxFrequencyLn, synthesisParameters.PointsCountAtFrequencyAxle, scheme.Model);

            //var t = scheme.DeepClone();


            int n = scheme.Model.FESections.Count;

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

            return ga.Population;
        }
    }
}
