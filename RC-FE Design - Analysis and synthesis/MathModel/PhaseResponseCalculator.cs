using MathNet.Numerics.LinearAlgebra;
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
    class PhaseResponseCalculator
    {
        // Метод для расчёта фазы
        public static double CalculatePhase(FElementScheme scheme, double frequency) 
        {
            int pinsCount = scheme.FESections.First().Pins.Count;

            // глобальная матрица y-параметров
            var Y = Matrix<Complex>.Build.DenseOfArray(new Complex[scheme.FESections.Count * pinsCount, scheme.FESections.Count * pinsCount]);
            // сформировать глобальную матрицу проводимости
            CreateGlobalConductivityMatrix(Y, pinsCount, frequency, scheme);

            // матрица соединений (инцидентности)
            var I = Matrix<float>.Build.DenseOfArray(new float[scheme.FESections.Count * pinsCount, scheme.FESections.Count * pinsCount]);
            // сформировать глобальную матрицу инцидентности
            CreateGlobalIncidenceMatrix(I, pinsCount, scheme);
            
            // номера заземлённых выводов
            var PE = new List<int>();
            // найти номера заземлённых выводов
            FindPEIndices(PE, scheme);

            RemoveRowAndColsFromMatrix(ref Y, PE);
            RemoveRowAndColsFromMatrix(ref I, PE);

            I.SetDiagonal(Vector<float>.Build.Dense(I.ColumnCount, 0));

            AddRowsAndColsInYMatrix(ref Y, ref I);

            // привести матрицу в соответствие с нумерацией выводов элементов
            // перестановка с учётом порядка обхода каждого элемента: левый верхний -> правый верхний -> правый нижний -> левый нижний
            Y.PermuteColumns(new MathNet.Numerics.Permutation(scheme.PinsNumbering)); 
            Y.PermuteRows(new MathNet.Numerics.Permutation(scheme.PinsNumbering));

            // TEST!!
            var test = Y.ToArray();

            double phase = 0;

            return phase;
        }


        // метод для сложения столбцов и строк соединённых выводов
        private static void AddRowsAndColsInYMatrix(ref Matrix<Complex> Y, ref Matrix<float> I) 
        {
            // обойти верхнюю треугольную матрицу
            for (int i = 0; i < I.RowCount; i++)
            {
                for (int j = i + 1; j < I.ColumnCount; j++)
                {
                    // сложить столбцы и строки соединённых выводов в матрице проводимости
                    if (I[i,j] == 1)
                    {
                        var firstRow = Y.Row(i);
                        var secondRow = Y.Row(j);
                        var rowSum = firstRow.Add(secondRow);

                        Y.SetRow(i, rowSum);

                        var firstCol = Y.Column(i);
                        var secondCol = Y.Column(j);
                        var colSum = firstCol.Add(secondCol);

                        Y.SetColumn(i, colSum);

                        Y = RemoveRowAndColsFromMatrix(Y, j);
                        I = RemoveRowAndColsFromMatrix(I, j);
                    }
                }
            }
        }

        // метод для удаления строк и столбцов из матрицы
        private static void RemoveRowAndColsFromMatrix<T>(ref Matrix<T> matrix, List<int> indices) where T : struct , IEquatable<T>, IFormattable
        {
            var result = matrix;

            foreach (var index in indices)
            {
                result = result.RemoveColumn(index);
                result = result.RemoveRow(index);
            }

            matrix = result;
        }

        // метод для удаления строки и столбца из матрицы
        private static Matrix<T> RemoveRowAndColsFromMatrix<T>(Matrix<T> matrix, int index) where T : struct, IEquatable<T>, IFormattable
        {
            var result = matrix;

            result = result.RemoveColumn(index);
            result = result.RemoveRow(index);          

            if (result != null)
                return result;
            else
                return matrix;
        }

        // Метод для поиска номеров заземлённых выводов
        private static void FindPEIndices(List<int> vector, FElementScheme scheme)
        {
            // для всех соединений схемы
            foreach (var connection in scheme.InnerConnections) 
            {
                var localVector = FElementScheme.AllowablePinsConnections[connection.ConnectionType].PEVector[connection.PEType];

                for (int i = 0; i < localVector.Length; i++)
                {
                    if (localVector[i] == 1)
                    {
                        int index = MapIndexToGlobal(i, connection);
                        vector.Add(index);
                    }
                }
            }
        }

        // Метод для создания матрицы проводимости
        private static void CreateGlobalConductivityMatrix(Matrix<Complex> matrix, int pinsCount, double frequency, FElementScheme scheme) 
        {
            // для всех секций фрэ
            for (int sectionNumber = 0; sectionNumber < scheme.FESections.Count; sectionNumber++)
            {
                var row_col_index = (sectionNumber) * pinsCount;

                var parameters = scheme.FESections[sectionNumber].SectionParameters;

                var thetaFunc = FESection.ThetaFunctions.Single(x => x.Key == parameters.SectionType).Value;
                var theta = thetaFunc(parameters.R, parameters.N, parameters.C, parameters.Rp, parameters.Rk, parameters.G, parameters.L, frequency);

                var th = Complex.Tanh(theta);
                var sh = Complex.Sinh(theta);

                var theta_div_th = (theta / th);
                var theta_div_sh = (theta / sh);

                var coeficient = 1 / ((1 + parameters.N) * parameters.R * parameters.L);

                var mattrix = scheme.FESections[sectionNumber].YParametersMatrix;

                // рассчитать матрицу БКЭ 
                mattrix[0, 0] = coeficient * (theta_div_th + parameters.N);
                mattrix[0, 1] = coeficient * (-theta_div_sh - parameters.N);
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
                matrix.SetSubMatrix(row_col_index, row_col_index, scheme.FESections[sectionNumber].YParametersMatrix);
            }
        }

        // Метод для создания матрицы инцидентности
        private static void CreateGlobalIncidenceMatrix(Matrix<float> matrix, int sectionPinsCount, FElementScheme scheme) 
        {
            // установить диагональ
            var diagonal = Vector<float>.Build.Dense(scheme.FESections.Count*sectionPinsCount, 1);
            matrix.SetDiagonal(diagonal);

            // для всех соединений схемы
            foreach (var connection in scheme.InnerConnections)
            {
                var localMatrix = FElementScheme.AllowablePinsConnections[connection.ConnectionType];
                var upperBound0 = localMatrix.ConnectionMatrix.GetUpperBound(0);
                var upperBound1 = localMatrix.ConnectionMatrix.GetUpperBound(1);

                // обойти матрицу инцидентности
                for (int i = 0; i <= upperBound0; i++)
                {
                    for (int j = i; j <= upperBound1; j++)
                    {
                        if (localMatrix.ConnectionMatrix[i,j] == 1)
                        {
                            int global_index_1 = MapIndexToGlobal(i, connection);
                            int global_index_2 = MapIndexToGlobal(j, connection);

                            matrix[global_index_1, global_index_2] = 1;
                            matrix[global_index_2, global_index_1] = 1;
                        }
                    }
                }
            }
        }

        // найти номер вывода в схеме по номеру вывода элемента
        private static int MapIndexToGlobal(int pin, Connection connection)
        {
            // спроецировать внутреннюю нумерацию шаблонного соединения на выводы двух БКЭ
            switch (pin)
            {
                case 0:
                    return connection.FirstSection.Pins[1].Number;
                case 3:
                    return connection.FirstSection.Pins[2].Number;
                case 1:
                    return connection.SecondSection.Pins[0].Number;
                case 2:
                    return connection.SecondSection.Pins[3].Number;
            }

            return -1;
        }
    }
}
