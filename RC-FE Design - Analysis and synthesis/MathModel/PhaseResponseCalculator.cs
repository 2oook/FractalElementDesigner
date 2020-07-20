using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Класс для расчета ФЧХ
    /// </summary>
    public class PhaseResponseCalculator
    {
        // Метод для расчёта фазы
        public static double CalculatePhase(FElementScheme scheme, double frequency) 
        {
            double phase = 0;

            var Y = new double[scheme.FESections.Count * scheme.PinsCount, scheme.FESections.Count * scheme.PinsCount];

            // для всех секций фрэ
            for (int i = 0; i < scheme.FESections.Count; i++)
            {
                var y_start_col = (i + 1) * scheme.PinsCount;
                var y_start_row = (i + 1) * scheme.PinsCount;

                var parameters = scheme.FESections[i].SectionParameters;

                var thetaFunc = FESection.ThetaFunctions.Single(x => x.Key == parameters.SectionType).Value;
                var theta = thetaFunc(parameters.R, parameters.N, parameters.C, parameters.Rp, parameters.Rk, parameters.G, parameters.L, frequency);

                var th = Math.Tanh(theta);
                var sh = Math.Sinh(theta);

                var coeficient = 1 / ((1 + parameters.N)*parameters.R);


            }

            return phase;
        }
    }
}
