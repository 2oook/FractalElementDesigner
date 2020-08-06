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

            IGeneticAlgorithm ga = new SingleStageGeneticAlgorithm(synthesisParameters, 100);

            ga.InitiatePopulation(scheme);

            double increment = 100f / synthesisParameters.CountOfWholeStepsOfGA;

            for (int i = 0; i < synthesisParameters.CountOfWholeStepsOfGA; i++)
            {
                OnDoWork(increment * (i + 1));

                // для сравнения мутировавших особей
                //var t = ga.Population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();

                ga.MutatePopulation();

                // для сравнения мутировавших особей
                //var t1 = ga.Population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();

                ga.CrossPopulation();

                //var t2 = ga.Population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();

                ga.RatePopulation();

                // для просмотра оценки 
                //var t3 = ga.Population.OrderByDescending(x => x.Model.Rate).Select(x => x.Model).ToList();

                ga.SelectPopulation();

                //var t99 = population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();
                //var t9 = population.Select(x => x.Model.InnerConnections.Select(y => y.PEType).ToList()).ToList();
            }

            // для примера
            // рассчитать ФЧХ для схемы
            scheme.Model.PhaseResponsePoints = InnerSchemePhaseResponseCalculator.CalculatePhaseResponseInScheme(
                synthesisParameters.MinFrequencyLn, synthesisParameters.MaxFrequencyLn, synthesisParameters.PointsCountAtFrequencyAxle, scheme.Model);
            // для примера
            var t = ga.GetPopulation();
            t.Add(scheme);
            // для примера

            OnStateChange("");

            return ga.GetPopulation();
        }
    }
}
