using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.RCWorkbenchLibrary.Helpers
{
    /// <summary>
    /// Помощник для адаптации матрицы из RCWorkbench в пригодную для обработки матрицу
    /// </summary>
    class MatrixHelper
    {
        // Метод для получения преобразованных матриц из векторов полученных из RC Workbench по частотам
        public static List<Matrix<Complex>> GetYParametersMatricesFromRCWorkbenchArray(double[,] y_parameters_double, int outer_pins_count)
        {
            var matrices = new List<Matrix<Complex>>();

            var first_dimension = y_parameters_double.GetUpperBound(0) + 1;
            var second_dimension = y_parameters_double.GetUpperBound(1) + 1;

            // для каждой матрицы представленной 36-ти местным вектором (по частотам)
            for (int i = 0; i < first_dimension; i++)
            {
                // новая матрица
                var y = Matrix<Complex>.Build.DenseOfArray(new Complex[outer_pins_count, outer_pins_count]);

                int j = 0; // один индекс для вектора

                // сначала k - число контактов // затем уменьшается на 1 каждую итерацию
                for (int k = outer_pins_count, row_index = 0; k > 0; k--, row_index++)
                {
                    for (int t = outer_pins_count - k; t < outer_pins_count; t++)
                    {
                        // собрать комплексное число и поместить его в матрицу 
                        y[row_index, t] = new Complex(y_parameters_double[i, j * 2], y_parameters_double[i, j * 2 + 1]);
                        j++;
                    }
                }

                matrices.Add(y);
            }

            return matrices;
        }
    }
}
