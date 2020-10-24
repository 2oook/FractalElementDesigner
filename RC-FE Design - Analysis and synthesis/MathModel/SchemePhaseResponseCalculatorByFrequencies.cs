﻿using MathNet.Numerics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Класс для расчёта ФЧХ схемы
    /// </summary>
    class SchemePhaseResponseCalculatorByFrequencies
    {
        // Метод для расчёта ФЧХ схемы
        public static List<(double frequency, double phase)> CalculatePhaseResponseInScheme(double minFrequencyLn, double maxFrequencyLn, double pointsCount, FESchemeModel scheme) 
        {
            var points = new List<(double frequency, double phase)>();

            var maxFrequency = Math.Pow(10, maxFrequencyLn);
            var minFrequency = Math.Pow(10, minFrequencyLn);

            double increment = (maxFrequency - minFrequency) / pointsCount;
            double frequency = minFrequency;

            // последовательная реализация
            // цикл по частотам
            for (int i = 0; i <= pointsCount; i++)
            {
                var phase = SchemePhaseResponseCalculator.CalculatePhase(scheme, frequency);

                points.Add((frequency, phase));

                frequency += increment;
            }

            return points;
        }
    }
}
