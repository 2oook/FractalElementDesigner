﻿using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using FractalElementDesigner.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
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
        public static List<FElementScheme> Synthesize(FElementScheme scheme) 
        {
            OnStateChange("Выполнение синтеза");

            IGeneticAlgorithm ga = new SingleStageGeneticAlgorithm(100, scheme);

            SingleStageGeneticAlgorithm.OnDoWork += SingleStageGeneticAlgorithm_OnDoWork;

            ga.Start();

            // для примера
            // рассчитать ФЧХ для схемы
            scheme.Model.PhaseResponsePoints = SchemePhaseResponseCalculatorByFrequencies.CalculatePhaseResponseInScheme(
                scheme.SynthesisParameters.MinFrequencyLn, 
                scheme.SynthesisParameters.MaxFrequencyLn, 
                scheme.SynthesisParameters.PointsCountAtFrequencyAxle, scheme.Model);
            // для примера
            var t = ga.GetPopulation();
            t.Add(scheme);
            // для примера

            OnStateChange("");

            return ga.GetPopulation();
        }

        // Обработчик выполнения работы ГА
        private static void SingleStageGeneticAlgorithm_OnDoWork(double obj)
        {
            OnDoWork(obj);
        }
    }
}
