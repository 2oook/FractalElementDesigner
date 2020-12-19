using FractalElementDesigner.FEEditing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    class PhaseResponseCalculatorForStructureForAllFrequencies
    {
        // Метод для расчёта ФЧХ схемы
        public static List<(double frequency, double phase)> CalculatePhaseResponseInStructure(double minFrequencyLn, double maxFrequencyLn, double pointsCount, RCStructureBase structure,
            StructurePhaseResponseCalculator сalculator)
        {
            var points = new List<(double frequency, double phase)>();

            var maxFrequency = Math.Pow(10, maxFrequencyLn);
            var minFrequency = Math.Pow(10, minFrequencyLn);

            double increment = (maxFrequency - minFrequency) / pointsCount;
            double frequency = minFrequency;
            // цикл по частотам
            for (int i = 0; i <= pointsCount; i++)
            {
                var phase = сalculator.CalculatePhase(structure, frequency);

                points.Add((frequency, phase));

                frequency += increment;
            }

            return points;
        }
    }
}
