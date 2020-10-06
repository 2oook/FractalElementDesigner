using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Класс для расчета ФЧХ схемы
    /// </summary>
    class SchemePhaseResponseCalculator
    {
        // Метод для расчёта фазы
        public static double CalculatePhase(FESchemeModel scheme, double frequency) 
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

            // привести матрицу в соответствие с нумерацией выводов элементов
            // перестановка с учётом порядка обхода каждого элемента: левый верхний -> правый верхний -> правый нижний -> левый нижний
            var permutation = new Permutation(scheme.PinsNumbering);
            Y.PermuteColumns(permutation); 
            Y.PermuteRows(permutation);

            I.SetDiagonal(Vector<float>.Build.Dense(I.ColumnCount, 0));

            // скорее всего в Matlab есть ошибка, которая не учитывает перестановку матрицы инцидениции
            //I.PermuteColumns(permutation);
            //I.PermuteRows(permutation);

            // найти номера заземлённых выводов
            var PE = FindPEIndices(scheme);

            RemoveRowAndColsFromMatrix(ref Y, PE);
            RemoveRowAndColsFromMatrix(ref I, PE);

            AddRowsAndColsInYMatrix(ref Y, ref I);

            ReduceMatrix(ref Y, 4);

            var phase = -Y[Y.RowCount-1, Y.ColumnCount-1].Phase * 180 / Math.PI;

            return phase;
        }

        // Метод для понижения порядка матрицы
        private static void ReduceMatrix(ref Matrix<Complex> matrix, int upTo)
        {
            Matrix<Complex> temp;

            for (int i = matrix.ColumnCount - 1; i >= upTo; i--)
            {
                var column = matrix.Column(i).SubVector(0, i).ToColumnMatrix();
                var row = matrix.Row(i).SubVector(0, i).ToRowMatrix();

                temp = matrix.RemoveColumn(i);
                temp = temp.RemoveRow(i);

                // проверка знаменателя на 0
                if (matrix[i, i].IsZero())
                    matrix[i, i] = 1;

                matrix = temp - (column * (1 / matrix[i, i]) * row);
            }
        }

        // метод для сложения столбцов и строк соединённых выводов
        private static void AddRowsAndColsInYMatrix(ref Matrix<Complex> Y, ref Matrix<float> I) 
        {
            var indicesForRemove = new List<int>();

            // обойти верхнюю треугольную матрицу
            for (int i = 0; i < I.RowCount; i++)
            {
                for (int j = i + 1; j < I.ColumnCount; j++)
                {
                    // сложить столбцы и строки соединённых выводов в матрице проводимости
                    if (I[i,j] == 1)
                    {
                        int _i = i;
                        int _j = j;

                        // i всегда меньше
                        if (i > j)
                        {
                            _i = j;
                            _j = i;
                        }

                        var firstRow = Y.Row(_i);
                        var secondRow = Y.Row(_j);
                        var rowSum = firstRow.Add(secondRow);

                        Y.SetRow(_i, rowSum);

                        var firstCol = Y.Column(_i);
                        var secondCol = Y.Column(_j);
                        var colSum = firstCol.Add(secondCol);

                        Y.SetColumn(_i, colSum);

                        if (!indicesForRemove.Contains(_j))
                        {
                            indicesForRemove.Add(_j);
                        }                   
                    }
                }
            }

            indicesForRemove.Sort();

            RemoveRowAndColsFromMatrix(ref Y, indicesForRemove);
            RemoveRowAndColsFromMatrix(ref I, indicesForRemove);
        }

        // метод для удаления строк и столбцов из матрицы
        private static void RemoveRowAndColsFromMatrix<T>(ref Matrix<T> matrix, List<int> indices) where T : struct , IEquatable<T>, IFormattable
        {
            // число для корректировки индекса в матрице, т.к. число столбцов и строк матрицы уменьшается во время работы цикла
            int adjustCounter;

            for (int i = 0; i < indices.Count; i++)
            {
                adjustCounter = i;

                var index = indices[i] - adjustCounter;

                matrix = matrix.RemoveColumn(index);
                matrix = matrix.RemoveRow(index);
            }
        }

        // метод для удаления строки и столбца из матрицы
        private static void RemoveRowAndColsFromMatrix<T>(ref Matrix<T> matrix, int index) where T : struct, IEquatable<T>, IFormattable
        {
            matrix = matrix.RemoveColumn(index);
            matrix = matrix.RemoveRow(index);
        }

        // Метод для поиска номеров заземлённых выводов
        private static List<int> FindPEIndices(FESchemeModel scheme)
        {
            // номера заземлённых выводов
            var vector = new List<int>();

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

            vector.Sort();

            return vector;
        }

        // Метод для создания матрицы проводимости
        private static void CreateGlobalConductivityMatrix(Matrix<Complex> matrix, int pinsCount, double frequency, FESchemeModel scheme) 
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
        private static void CreateGlobalIncidenceMatrix(Matrix<float> matrix, int sectionPinsCount, FESchemeModel scheme) 
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
