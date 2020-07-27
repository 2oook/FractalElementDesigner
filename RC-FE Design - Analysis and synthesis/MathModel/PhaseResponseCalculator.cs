﻿using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

            int pinsCount = scheme.FESections.First().Pins.Count;

            // глобальная матрица y-параметров
            var Y = Matrix<Complex>.Build.DenseOfArray(new Complex[scheme.FESections.Count * pinsCount, scheme.FESections.Count * pinsCount]);

            // для всех секций фрэ
            for (int sectionNumber = 0; sectionNumber < scheme.FESections.Count; sectionNumber++)
            {
                var row_col_index = (sectionNumber) * pinsCount;

                var parameters = scheme.FESections[sectionNumber].SectionParameters;

                var thetaFunc = FESection.ThetaFunctions.Single(x => x.Key == parameters.SectionType).Value;
                var theta = thetaFunc(parameters.R, parameters.N, parameters.C, parameters.Rp, parameters.Rk, parameters.G, parameters.L, frequency);

                var th = Complex.Tanh(theta);
                var sh = Complex.Sinh(theta);

                var theta_div_th = (theta/th);
                var theta_div_sh = (theta /sh);

                var coeficient = 1 / ((1 + parameters.N)*parameters.R*parameters.L);

                var mattrix = scheme.FESections[sectionNumber].YParametersMatrix;

                // рассчитать матрицу БКЭ 
                mattrix[0, 0] = coeficient * (theta_div_th + parameters.N);
                mattrix[0, 1] = coeficient * (- theta_div_sh - parameters.N);
                mattrix[0, 2] = coeficient * (theta_div_sh - 1);
                mattrix[0, 3] = coeficient * (1 - theta_div_th);

                mattrix[1, 0] = mattrix[0, 1];
                mattrix[1, 1] = mattrix[0, 0];
                mattrix[1, 2] = mattrix[0, 3];
                mattrix[1, 3] = mattrix[0, 2];

                mattrix[2, 0] = mattrix[0, 2];
                mattrix[2, 1] = mattrix[1, 2];
                mattrix[2, 2] = coeficient * (theta_div_th + 1 / parameters.N);
                mattrix[2, 3] = coeficient * (-theta_div_sh - 1 / parameters.N);

                mattrix[3, 0] = mattrix[0, 3];
                mattrix[3, 1] = mattrix[1, 3];
                mattrix[3, 2] = mattrix[2, 3];
                mattrix[3, 3] = mattrix[2, 2];

                // скопировать матрицу БКЭ в глобальную матрицу
                Y.SetSubMatrix(row_col_index, row_col_index, scheme.FESections[sectionNumber].YParametersMatrix);
            }

            // матрица соединений (инциденции)
            var I = Matrix<float>.Build.DenseOfArray(new float[scheme.FESections.Count * pinsCount, scheme.FESections.Count * pinsCount]);

            // скопировать матрицу инциденции в глобальную матрицу инциденций
            CreateGlobalIncidenceMatrix(I, pinsCount, scheme);

            // привести матрицу в соответствие с нумерацией выводов элементов
            // перестановка с учётом порядка обхода каждого элемента: левый верхний -> правый верхний -> правый нижний -> левый нижний
            Y.PermuteColumns(new MathNet.Numerics.Permutation(scheme.PinsNumbering)); 
            Y.PermuteRows(new MathNet.Numerics.Permutation(scheme.PinsNumbering));

            // TEST!!
            var test = Y.ToArray();

            return phase;
        }

        private static void CreateGlobalIncidenceMatrix(Matrix<float> matrix, int sectionPinsCount, FElementScheme scheme) 
        {
            // для всех соединений схемы
            for (int connectionNumber = 0; connectionNumber < scheme.InnerConnections.Count; connectionNumber++)
            {
                var row_col_index = (connectionNumber) * sectionPinsCount;

                if (row_col_index == 0)
                {
                    // первые четыре вывода не подключены
                    var firstMatrix = Matrix<float>.Build.DenseOfArray(new float[sectionPinsCount, sectionPinsCount]);
                    var diagonal = Vector<float>.Build.Dense(sectionPinsCount, 1);
                    firstMatrix.SetDiagonal(diagonal);
                    matrix.SetSubMatrix(row_col_index, row_col_index, firstMatrix);

                    return;
                }

            }


        }
    }
}
