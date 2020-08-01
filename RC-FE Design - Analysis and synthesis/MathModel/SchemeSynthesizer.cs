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

            var points = new List<(double, double)>();

            double increment = (synthesisParameters.MaxFrequency - synthesisParameters.MinFrequency)/ synthesisParameters.PointsCountAtFrequencyAxle;
            double frequency = synthesisParameters.MinFrequency;
            // цикл по частотам
            for (int i = 0; i < synthesisParameters.PointsCountAtFrequencyAxle; i++)
            {
                var phase = PhaseResponseCalculator.CalculatePhase(scheme, Math.Pow(10,frequency));

                points.Add((Math.Pow(10, frequency), phase));

                frequency += increment;
            }

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

            var plot = scheme.Plots.SingleOrDefault();

            if (plot != null)
            {
                var series = new LineSeries() { InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline };

                foreach (var point in points)
                {
                    series.Points.Add(new DataPoint(point.Item1, point.Item2));
                }


                //series.Points.Add(new DataPoint(0.1, -6));
                //series.Points.Add(new DataPoint(1, -22));
                //series.Points.Add(new DataPoint(10, -20));
                //series.Points.Add(new DataPoint(100, -15));

                plot.Model = new PlotModel() { Title = "ФЧХ" };

                plot.Model.Axes.Add(new LinearAxis() { Title = "φ", Position = AxisPosition.Left, Unit = "град" , AxisTitleDistance = 10 });
                plot.Model.Axes.Add(new LogarithmicAxis() { Title = "ωRC", Position = AxisPosition.Bottom, Base = 10, Minimum = 0.1 });

                plot.Model.Series.Add(series);
            }

            // для отладки
            // для отладки
            // для отладки

            OnStateChange("");

            return result;
        }
    }
}
