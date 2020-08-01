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
        public static List<(double frequency, double phase)> CalculatePhaseResponseInScheme(double minFrequency, double maxFrequency, double pointsCount, FElementScheme scheme) 
        {
            var points = new List<(double frequency, double phase)>();

            double increment = (maxFrequency - minFrequency) / pointsCount;
            double frequency = minFrequency;
            // цикл по частотам
            for (int i = 0; i < pointsCount; i++)
            {
                var f = Math.Pow(10, frequency);
                var phase = PhaseResponseCalculator.CalculatePhase(scheme, f);

                points.Add((f, phase));

                frequency += increment;
            }

            return points;
        }
    }
}
