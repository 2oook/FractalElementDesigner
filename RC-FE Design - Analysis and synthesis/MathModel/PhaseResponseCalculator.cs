using MathNet.Numerics.LinearAlgebra;
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

            int pinsCount = scheme.FESections.First().SectionParameters.PinsCount;

            // глобальная матрица y-параметров
            var Y = Matrix<double>.Build.DenseOfArray(new double[scheme.FESections.Count * pinsCount, scheme.FESections.Count * pinsCount]);

            // для всех секций фрэ
            for (int sectionNumber = 0; sectionNumber < scheme.FESections.Count; sectionNumber++)
            {
                var y_start_col = (sectionNumber) * pinsCount;
                var y_start_row = (sectionNumber) * pinsCount;

                var parameters = scheme.FESections[sectionNumber].SectionParameters;

                var thetaFunc = FESection.ThetaFunctions.Single(x => x.Key == parameters.SectionType).Value;
                var theta = thetaFunc(parameters.R, parameters.N, parameters.C, parameters.Rp, parameters.Rk, parameters.G, parameters.L, frequency);

                var th = Math.Tanh(theta);
                var sh = Math.Sinh(theta);

                var coeficient = 1 / ((1 + parameters.N)*parameters.R);

                // скопировать элементы матриц БКЭ в глобальную матрицу
                for (int i = y_start_col, inner_i = 0; i < scheme.FESections[sectionNumber].YParametersMatrix.ColumnCount + y_start_col; i++, inner_i++)
                {
                    for (int j = y_start_row, inner_j = 0; j < scheme.FESections[sectionNumber].YParametersMatrix.RowCount + y_start_row; j++, inner_j++)
                    {
                        // поместить в матрицу y- параметров секции  
                        scheme.FESections[sectionNumber].YParametersMatrix[inner_i, inner_j] = sectionNumber+1;
                        Y[i, j] = scheme.FESections[sectionNumber].YParametersMatrix[inner_i, inner_j];
                    }
                }
            }

            // привести матрицу в соответствие с нумерацией выводов элементов
            Y.PermuteColumns(new MathNet.Numerics.Permutation(scheme.PinsNumbering));
            Y.PermuteRows(new MathNet.Numerics.Permutation(scheme.PinsNumbering));

            // TEST!!
            var test = Y.ToArray();
            return phase;
        }
    }
}
