using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Класс для расчёта ФЧХ схемы
    /// </summary>
    class InnerSchemePhaseResponseCalculator
    {
        // Метод для расчёта ФЧХ схемы
        public static List<(double frequency, double phase)> CalculatePhaseResponseInScheme(double minFrequencyLn, double maxFrequencyLn, double pointsCount, FElementScheme scheme) 
        {
            var points = new List<(double frequency, double phase)>();

            var maxFrequency = Math.Pow(10, maxFrequencyLn);
            var minFrequency = Math.Pow(10, minFrequencyLn);

            double increment = (maxFrequency - minFrequency) / pointsCount;
            double frequency = minFrequency;
            // цикл по частотам
            for (int i = 0; i <= pointsCount; i++)
            {
                var phase = PhaseResponseCalculator.CalculatePhase(scheme, frequency);

                points.Add((frequency, phase));

                frequency += increment;
            }

            return points;
        }
    }
}
