using FractalElementDesigner.FEEditing.Model;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FractalElementDesigner.MathModel.Structure
{
    /// <summary>
    /// Класс для расчета ФЧХ конструкции
    /// </summary>
    class StructurePhaseResponseCalculator
    {
        public StructurePhaseResponseCalculator(RCStructureBase structure, int horizontalDimension, int verticalDimension, int nodesCount, int[,,] nodesNumeration)
        {
            Structure = structure;

            NodesCount = nodesCount;
            NodesNumeration = nodesNumeration;

            HorizontalDimension = horizontalDimension;
            VerticalDimension = verticalDimension;

            // получить номера узлов для сложения и удаления  строк и столбцов глобальной матрицы
            PEPinsAndConnectedPinsIndices = FindPEPinsAndConnectedPinsIndices(Structure);

            GroundedOuterPins = SchemePhaseResponseCalculator.FindGroundedOuterPinsNumbers(Structure.Scheme.Model);
            ConnectedOuterPinsMatrix = SchemePhaseResponseCalculator.FindConnectedOuterPinsNumbers(Structure.Scheme.Model, Structure.Scheme.Model.OuterPins.Count, Structure.Scheme.Model.OuterPins.Count);
        }

        /// <summary>
        /// Ссылка на конструкцию
        /// </summary>
        RCStructureBase Structure;

        /// <summary>
        /// Количество ячеек по горизонтали (без учета контактных)
        /// </summary>
        int HorizontalDimension;

        /// <summary>
        /// Количество ячеек по вертикали (без учета контактных)
        /// </summary>
        int VerticalDimension;

        /// <summary>
        /// Число узлов
        /// </summary>
        int NodesCount;

        /// <summary>
        /// Нумерация узлов
        /// </summary>
        int[,,] NodesNumeration;

        /// <summary>
        /// Глобальная матрица проводимости
        /// </summary>
        Matrix<Complex> GlobalY;

        /// <summary>
        /// Заземленные и соединённые выводы
        /// </summary>
        (List<int> PE, Matrix<float> I) PEPinsAndConnectedPinsIndices;

        /// <summary>
        /// Заземлённые внешние выводы
        /// </summary>
        List<int> GroundedOuterPins;

        /// <summary>
        /// Соединённые внешние выводы
        /// </summary>
        Matrix<float> ConnectedOuterPinsMatrix;

        // Метод для заполнения глобальной матрицы проводимости
        public void FillGlobalMatrix(RCStructureBase structure, int nodesCount, int[,,] nodesNumeration, double w) 
        {
            var x = HorizontalDimension;
            var y = VerticalDimension;

            var R = structure.SynthesisParameters.FESections.First().SectionParameters.R;
            var N = structure.SynthesisParameters.FESections.First().SectionParameters.N;
            var C = structure.SynthesisParameters.FESections.First().SectionParameters.C;
            var G = structure.SynthesisParameters.FESections.First().SectionParameters.G;
            var H = 100;//???????

            #region Mat_r

            double[,] Mat_r = new double[8,8];

            double RZ_x = R * 2.0;
            double RZ_y = R * 2.0;

            double Yrx = 1.0 / RZ_x;
            double Yry = 1.0 / RZ_y;

            Mat_r[0,0] = Yrx + Yry; Mat_r[0,1] = -Yrx; Mat_r[0,2] = -Yry; Mat_r[0,3] = 0.0; Mat_r[0,4] = 0.0; Mat_r[0,5] = 0.0; Mat_r[0,6] = 0.0; Mat_r[0,7] = 0.0;
            Mat_r[1,0] = Mat_r[0,1]; Mat_r[1,1] = Mat_r[0,0]; Mat_r[1,2] = 0.0; Mat_r[1,3] = Mat_r[0,2]; Mat_r[1,4] = 0.0; Mat_r[1,5] = 0.0; Mat_r[1,6] = 0.0; Mat_r[1,7] = 0.0;
            Mat_r[2,0] = Mat_r[0,2]; Mat_r[2,1] = 0.0; Mat_r[2,2] = Mat_r[0,0]; Mat_r[2,3] = Mat_r[0,1]; Mat_r[2,4] = 0.0; Mat_r[2,5] = 0.0; Mat_r[2,6] = 0.0; Mat_r[2,7] = 0.0;
            Mat_r[3,0] = 0.0; Mat_r[3,1] = Mat_r[0,2]; Mat_r[3,2] = Mat_r[0,1]; Mat_r[3,3] = Mat_r[0,0]; Mat_r[3,4] = 0.0; Mat_r[3,5] = 0.0; Mat_r[3,6] = 0.0; Mat_r[3,7] = 0.0;
            Mat_r[4,0] = 0.0; Mat_r[4,1] = 0.0; Mat_r[4,2] = 0.0; Mat_r[4,3] = 0.0; Mat_r[4,4] = 0.0; Mat_r[4,5] = 0.0; Mat_r[4,6] = 0.0; Mat_r[4,7] = 0.0;
            Mat_r[5,0] = 0.0; Mat_r[5,1] = 0.0; Mat_r[5,2] = 0.0; Mat_r[5,3] = 0.0; Mat_r[5,4] = 0.0; Mat_r[5,5] = 0.0; Mat_r[5,6] = 0.0; Mat_r[5,7] = 0.0;
            Mat_r[6,0] = 0.0; Mat_r[6,1] = 0.0; Mat_r[6,2] = 0.0; Mat_r[6,3] = 0.0; Mat_r[6,4] = 0.0; Mat_r[6,5] = 0.0; Mat_r[6,6] = 0.0; Mat_r[6,7] = 0.0;
            Mat_r[7,0] = 0.0; Mat_r[7,1] = 0.0; Mat_r[7,2] = 0.0; Mat_r[7,3] = 0.0; Mat_r[7,4] = 0.0; Mat_r[7,5] = 0.0; Mat_r[7,6] = 0.0; Mat_r[7,7] = 0.0;

            #endregion

            #region Mat_nr

            double[,] Mat_nr = new double[8,8];

            double NZ_x = R * N * 2.0;
            double NZ_y = R * N * 2.0;

            double Ynrx = 1.0 / NZ_x;
            double Ynry = 1.0 / NZ_y;

            Mat_nr[0,0] = 0.0; Mat_nr[0,1] = 0.0; Mat_nr[0,2] = 0.0; Mat_nr[0,3] = 0.0; Mat_nr[0,4] = 0.0; Mat_nr[0,5] = 0.0; Mat_nr[0,6] = 0.0; Mat_nr[0,7] = 0.0;
            Mat_nr[1,0] = 0.0; Mat_nr[1,1] = 0.0; Mat_nr[1,2] = 0.0; Mat_nr[1,3] = 0.0; Mat_nr[1,4] = 0.0; Mat_nr[1,5] = 0.0; Mat_nr[1,6] = 0.0; Mat_nr[1,7] = 0.0;
            Mat_nr[2,0] = 0.0; Mat_nr[2,1] = 0.0; Mat_nr[2,2] = 0.0; Mat_nr[2,3] = 0.0; Mat_nr[2,4] = 0.0; Mat_nr[2,5] = 0.0; Mat_nr[2,6] = 0.0; Mat_nr[2,7] = 0.0;
            Mat_nr[3,0] = 0.0; Mat_nr[3,1] = 0.0; Mat_nr[3,2] = 0.0; Mat_nr[3,3] = 0.0; Mat_nr[3,4] = 0.0; Mat_nr[3,5] = 0.0; Mat_nr[3,6] = 0.0; Mat_nr[3,7] = 0.0;
            Mat_nr[4,0] = 0.0; Mat_nr[4,1] = 0.0; Mat_nr[4,2] = 0.0; Mat_nr[4,3] = 0.0; Mat_nr[4,4] = Ynrx + Ynry; Mat_nr[4,5] = -Ynrx; Mat_nr[4,6] = -Ynry; Mat_nr[4,7] = 0.0;
            Mat_nr[5,0] = 0.0; Mat_nr[5,1] = 0.0; Mat_nr[5,2] = 0.0; Mat_nr[5,3] = 0.0; Mat_nr[5,4] = Mat_nr[4,5]; Mat_nr[5,5] = Mat_nr[4,4]; Mat_nr[5,6] = 0.0; Mat_nr[5,7] = Mat_nr[4,6];
            Mat_nr[6,0] = 0.0; Mat_nr[6,1] = 0.0; Mat_nr[6,2] = 0.0; Mat_nr[6,3] = 0.0; Mat_nr[6,4] = Mat_nr[4,6]; Mat_nr[6,5] = 0.0; Mat_nr[6,6] = Mat_nr[4,4]; Mat_nr[6,7] = Mat_nr[4,5];
            Mat_nr[7,0] = 0.0; Mat_nr[7,1] = 0.0; Mat_nr[7,2] = 0.0; Mat_nr[7,3] = 0.0; Mat_nr[7,4] = 0.0; Mat_nr[7,5] = Mat_nr[4,6]; Mat_nr[7,6] = Mat_nr[4,5]; Mat_nr[7,7] = Mat_nr[4,4];

            #endregion

            #region RkCNRk

            Complex[,] Mat_Rk_C_NRk = new Complex[8,8]; // Y - параметры Rk_C_NRk - ячейки

            double Z_x_Rk_C_NRk = R * 2.0 * (1.0 + N);
            double Z_y_Rk_C_NRk = R * 2.0 * (1.0 + N);

            Complex p_Rk_C_NRk = new Complex(0.0, w);
            Complex temp_Rk_C_NRk = 1.0 / H + p_Rk_C_NRk * R * C;
            Complex Y_Rk_C_NRk = temp_Rk_C_NRk / ((1.0 + G * temp_Rk_C_NRk) * ((double)(4 * x * y)) * R);

            Complex tetta_x_Rk_C_NRk = Complex.Sqrt(Z_x_Rk_C_NRk * Y_Rk_C_NRk);
            Complex tetta_y_Rk_C_NRk = Complex.Sqrt(Z_y_Rk_C_NRk * Y_Rk_C_NRk);

            double koeff_x_Rk_C_NRk = 1.0 / Z_x_Rk_C_NRk;
            double koeff_y_Rk_C_NRk = 1.0 / Z_y_Rk_C_NRk;

            Complex tx_Rk_C_NRk = tetta_x_Rk_C_NRk / Complex.Tanh(tetta_x_Rk_C_NRk);
            Complex sx_Rk_C_NRk = tetta_x_Rk_C_NRk / Complex.Sinh(tetta_x_Rk_C_NRk);
            Complex ty_Rk_C_NRk = tetta_y_Rk_C_NRk / Complex.Tanh(tetta_y_Rk_C_NRk);
            Complex sy_Rk_C_NRk = tetta_y_Rk_C_NRk / Complex.Sinh(tetta_y_Rk_C_NRk);

            tx_Rk_C_NRk *= koeff_x_Rk_C_NRk;
            sx_Rk_C_NRk *= koeff_x_Rk_C_NRk;
            ty_Rk_C_NRk *= koeff_y_Rk_C_NRk;
            sy_Rk_C_NRk *= koeff_y_Rk_C_NRk;

            double KxN_Rk_C_NRk = koeff_x_Rk_C_NRk * N;
            double KyN_Rk_C_NRk = koeff_y_Rk_C_NRk * N;
            double Kx_N_Rk_C_NRk = koeff_x_Rk_C_NRk / N;
            double Ky_N_Rk_C_NRk = koeff_y_Rk_C_NRk / N;

            Mat_Rk_C_NRk[0,0] = tx_Rk_C_NRk + ty_Rk_C_NRk + KxN_Rk_C_NRk + KyN_Rk_C_NRk; Mat_Rk_C_NRk[0,1] = -sx_Rk_C_NRk - KxN_Rk_C_NRk; Mat_Rk_C_NRk[0,2] = -sy_Rk_C_NRk - KyN_Rk_C_NRk; Mat_Rk_C_NRk[0,3] = 0.0; Mat_Rk_C_NRk[0,4] = koeff_x_Rk_C_NRk + koeff_y_Rk_C_NRk - tx_Rk_C_NRk - ty_Rk_C_NRk; Mat_Rk_C_NRk[0,5] = sx_Rk_C_NRk - koeff_x_Rk_C_NRk; Mat_Rk_C_NRk[0,6] = sy_Rk_C_NRk - koeff_y_Rk_C_NRk; Mat_Rk_C_NRk[0,7] = 0.0;
            Mat_Rk_C_NRk[1,0] = Mat_Rk_C_NRk[0,1]; Mat_Rk_C_NRk[1,1] = Mat_Rk_C_NRk[0,0]; Mat_Rk_C_NRk[1,2] = 0.0; Mat_Rk_C_NRk[1,3] = Mat_Rk_C_NRk[0,2]; Mat_Rk_C_NRk[1,4] = Mat_Rk_C_NRk[0,5]; Mat_Rk_C_NRk[1,5] = Mat_Rk_C_NRk[0,4]; Mat_Rk_C_NRk[1,6] = 0.0; Mat_Rk_C_NRk[1,7] = Mat_Rk_C_NRk[0,6];
            Mat_Rk_C_NRk[2,0] = Mat_Rk_C_NRk[0,2]; Mat_Rk_C_NRk[2,1] = 0.0; Mat_Rk_C_NRk[2,2] = Mat_Rk_C_NRk[0,0]; Mat_Rk_C_NRk[2,3] = Mat_Rk_C_NRk[0,1]; Mat_Rk_C_NRk[2,4] = Mat_Rk_C_NRk[0,6]; Mat_Rk_C_NRk[2,5] = 0.0; Mat_Rk_C_NRk[2,6] = Mat_Rk_C_NRk[0,4]; Mat_Rk_C_NRk[2,7] = Mat_Rk_C_NRk[0,5];
            Mat_Rk_C_NRk[3,0] = 0.0; Mat_Rk_C_NRk[3,1] = Mat_Rk_C_NRk[0,2]; Mat_Rk_C_NRk[3,2] = Mat_Rk_C_NRk[0,1]; Mat_Rk_C_NRk[3,3] = Mat_Rk_C_NRk[0,0]; Mat_Rk_C_NRk[3,4] = 0.0; Mat_Rk_C_NRk[3,5] = Mat_Rk_C_NRk[0,6]; Mat_Rk_C_NRk[3,6] = Mat_Rk_C_NRk[0,5]; Mat_Rk_C_NRk[3,7] = Mat_Rk_C_NRk[0,4];
            Mat_Rk_C_NRk[4,0] = Mat_Rk_C_NRk[0,4]; Mat_Rk_C_NRk[4,1] = Mat_Rk_C_NRk[0,5]; Mat_Rk_C_NRk[4,2] = Mat_Rk_C_NRk[0,6]; Mat_Rk_C_NRk[4,3] = 0.0; Mat_Rk_C_NRk[4,4] = tx_Rk_C_NRk + ty_Rk_C_NRk + Kx_N_Rk_C_NRk + Ky_N_Rk_C_NRk; Mat_Rk_C_NRk[4,5] = -sx_Rk_C_NRk - Kx_N_Rk_C_NRk; Mat_Rk_C_NRk[4,6] = -sy_Rk_C_NRk - Ky_N_Rk_C_NRk; Mat_Rk_C_NRk[4,7] = 0.0;
            Mat_Rk_C_NRk[5,0] = Mat_Rk_C_NRk[0,5]; Mat_Rk_C_NRk[5,1] = Mat_Rk_C_NRk[0,4]; Mat_Rk_C_NRk[5,2] = 0.0; Mat_Rk_C_NRk[5,3] = Mat_Rk_C_NRk[0,6]; Mat_Rk_C_NRk[5,4] = Mat_Rk_C_NRk[4,5]; Mat_Rk_C_NRk[5,5] = Mat_Rk_C_NRk[4,4]; Mat_Rk_C_NRk[5,6] = 0.0; Mat_Rk_C_NRk[5,7] = Mat_Rk_C_NRk[4,6];
            Mat_Rk_C_NRk[6,0] = Mat_Rk_C_NRk[0,6]; Mat_Rk_C_NRk[6,1] = 0.0; Mat_Rk_C_NRk[6,2] = Mat_Rk_C_NRk[0,4]; Mat_Rk_C_NRk[6,3] = Mat_Rk_C_NRk[0,5]; Mat_Rk_C_NRk[6,4] = Mat_Rk_C_NRk[4,6]; Mat_Rk_C_NRk[6,5] = 0.0; Mat_Rk_C_NRk[6,6] = Mat_Rk_C_NRk[4,4]; Mat_Rk_C_NRk[6,7] = Mat_Rk_C_NRk[4,5];
            Mat_Rk_C_NRk[7,0] = 0.0; Mat_Rk_C_NRk[7,1] = Mat_Rk_C_NRk[0,6]; Mat_Rk_C_NRk[7,2] = Mat_Rk_C_NRk[0,5]; Mat_Rk_C_NRk[7,3] = Mat_Rk_C_NRk[0,4]; Mat_Rk_C_NRk[7,4] = 0.0; Mat_Rk_C_NRk[7,5] = Mat_Rk_C_NRk[4,6]; Mat_Rk_C_NRk[7,6] = Mat_Rk_C_NRk[4,5]; Mat_Rk_C_NRk[7,7] = Mat_Rk_C_NRk[4,4];

            #endregion

            #region RkCNR

            Complex[,] Mat_Rk_C_NR = new Complex[8,8]; // Y - параметры Rk_C_NR - ячейки

            double Z_x_Rk_C_NR = R * 2.0 * (1.0 + N);
            double Z_y_Rk_C_NR = R * 2.0 * (1.0 + N);

            Complex p_Rk_C_NR = new Complex(0.0, w);
            Complex temp_Rk_C_NR = 1.0 / H + p_Rk_C_NRk * R * C;
            Complex Y_Rk_C_NR = temp_Rk_C_NR / ((1.0 + G * temp_Rk_C_NR) * ((double)(4 * x * y)) * R);

            Complex tetta_x_Rk_C_NR = Complex.Sqrt(Z_x_Rk_C_NR * Y_Rk_C_NR);
            Complex tetta_y_Rk_C_NR = Complex.Sqrt(Z_y_Rk_C_NR * Y_Rk_C_NR);

            double koeff_x_Rk_C_NR = 1.0 / Z_x_Rk_C_NR;
            double koeff_y_Rk_C_NR = 1.0 / Z_y_Rk_C_NR;

            Complex tx_Rk_C_NR = tetta_x_Rk_C_NR / Complex.Tanh(tetta_x_Rk_C_NR);
            Complex sx_Rk_C_NR = tetta_x_Rk_C_NR / Complex.Sinh(tetta_x_Rk_C_NR);
            Complex ty_Rk_C_NR = tetta_y_Rk_C_NR / Complex.Tanh(tetta_y_Rk_C_NR);
            Complex sy_Rk_C_NR = tetta_y_Rk_C_NR / Complex.Sinh(tetta_y_Rk_C_NR);

            tx_Rk_C_NR *= koeff_x_Rk_C_NR;
            sx_Rk_C_NR *= koeff_x_Rk_C_NR;
            ty_Rk_C_NR *= koeff_y_Rk_C_NR;
            sy_Rk_C_NR *= koeff_y_Rk_C_NR;

            double KxN_Rk_C_NR = koeff_x_Rk_C_NR * N;
            double KyN_Rk_C_NR = koeff_y_Rk_C_NR * N;
            double Kx_N_Rk_C_NR = koeff_x_Rk_C_NR / N;
            double Ky_N_Rk_C_NR = koeff_y_Rk_C_NR / N;

            Mat_Rk_C_NR[0,0] = tx_Rk_C_NR + ty_Rk_C_NR + KxN_Rk_C_NR + KyN_Rk_C_NR; Mat_Rk_C_NR[0,1] = -sx_Rk_C_NR - KxN_Rk_C_NR; Mat_Rk_C_NR[0,2] = -sy_Rk_C_NR - KyN_Rk_C_NR; Mat_Rk_C_NR[0,3] = 0.0; Mat_Rk_C_NR[0,4] = koeff_x_Rk_C_NR + koeff_y_Rk_C_NR - tx_Rk_C_NR - ty_Rk_C_NR; Mat_Rk_C_NR[0,5] = sx_Rk_C_NR - koeff_x_Rk_C_NR; Mat_Rk_C_NR[0,6] = sy_Rk_C_NR - koeff_y_Rk_C_NR; Mat_Rk_C_NR[0,7] = 0.0;
            Mat_Rk_C_NR[1,0] = Mat_Rk_C_NR[0,1]; Mat_Rk_C_NR[1,1] = Mat_Rk_C_NR[0,0]; Mat_Rk_C_NR[1,2] = 0.0; Mat_Rk_C_NR[1,3] = Mat_Rk_C_NR[0,2]; Mat_Rk_C_NR[1,4] = Mat_Rk_C_NR[0,5]; Mat_Rk_C_NR[1,5] = Mat_Rk_C_NR[0,4]; Mat_Rk_C_NR[1,6] = 0.0; Mat_Rk_C_NR[1,7] = Mat_Rk_C_NR[0,6];
            Mat_Rk_C_NR[2,0] = Mat_Rk_C_NR[0,2]; Mat_Rk_C_NR[2,1] = 0.0; Mat_Rk_C_NR[2,2] = Mat_Rk_C_NR[0,0]; Mat_Rk_C_NR[2,3] = Mat_Rk_C_NR[0,1]; Mat_Rk_C_NR[2,4] = Mat_Rk_C_NR[0,6]; Mat_Rk_C_NR[2,5] = 0.0; Mat_Rk_C_NR[2,6] = Mat_Rk_C_NR[0,4]; Mat_Rk_C_NR[2,7] = Mat_Rk_C_NR[0,5];
            Mat_Rk_C_NR[3,0] = 0.0; Mat_Rk_C_NR[3,1] = Mat_Rk_C_NR[0,2]; Mat_Rk_C_NR[3,2] = Mat_Rk_C_NR[0,1]; Mat_Rk_C_NR[3,3] = Mat_Rk_C_NR[0,0]; Mat_Rk_C_NR[3,4] = 0.0; Mat_Rk_C_NR[3,5] = Mat_Rk_C_NR[0,6]; Mat_Rk_C_NR[3,6] = Mat_Rk_C_NR[0,5]; Mat_Rk_C_NR[3,7] = Mat_Rk_C_NR[0,4];
            Mat_Rk_C_NR[4,0] = Mat_Rk_C_NR[0,4]; Mat_Rk_C_NR[4,1] = Mat_Rk_C_NR[0,5]; Mat_Rk_C_NR[4,2] = Mat_Rk_C_NR[0,6]; Mat_Rk_C_NR[4,3] = 0.0; Mat_Rk_C_NR[4,4] = tx_Rk_C_NR + ty_Rk_C_NR + Kx_N_Rk_C_NR + Ky_N_Rk_C_NR; Mat_Rk_C_NR[4,5] = -sx_Rk_C_NR - Kx_N_Rk_C_NR; Mat_Rk_C_NR[4,6] = -sy_Rk_C_NR - Ky_N_Rk_C_NR; Mat_Rk_C_NR[4,7] = 0.0;
            Mat_Rk_C_NR[5,0] = Mat_Rk_C_NR[0,5]; Mat_Rk_C_NR[5,1] = Mat_Rk_C_NR[0,4]; Mat_Rk_C_NR[5,2] = 0.0; Mat_Rk_C_NR[5,3] = Mat_Rk_C_NR[0,6]; Mat_Rk_C_NR[5,4] = Mat_Rk_C_NR[4,5]; Mat_Rk_C_NR[5,5] = Mat_Rk_C_NR[4,4]; Mat_Rk_C_NR[5,6] = 0.0; Mat_Rk_C_NR[5,7] = Mat_Rk_C_NR[4,6];
            Mat_Rk_C_NR[6,0] = Mat_Rk_C_NR[0,6]; Mat_Rk_C_NR[6,1] = 0.0; Mat_Rk_C_NR[6,2] = Mat_Rk_C_NR[0,4]; Mat_Rk_C_NR[6,3] = Mat_Rk_C_NR[0,5]; Mat_Rk_C_NR[6,4] = Mat_Rk_C_NR[4,6]; Mat_Rk_C_NR[6,5] = 0.0; Mat_Rk_C_NR[6,6] = Mat_Rk_C_NR[4,4]; Mat_Rk_C_NR[6,7] = Mat_Rk_C_NR[4,5];
            Mat_Rk_C_NR[7,0] = 0.0; Mat_Rk_C_NR[7,1] = Mat_Rk_C_NR[0,6]; Mat_Rk_C_NR[7,2] = Mat_Rk_C_NR[0,5]; Mat_Rk_C_NR[7,3] = Mat_Rk_C_NR[0,4]; Mat_Rk_C_NR[7,4] = 0.0; Mat_Rk_C_NR[7,5] = Mat_Rk_C_NR[4,6]; Mat_Rk_C_NR[7,6] = Mat_Rk_C_NR[4,5]; Mat_Rk_C_NR[7,7] = Mat_Rk_C_NR[4,4];

            #endregion

            #region RCNRk

            Complex[,] Mat_R_C_NRk = new Complex[8,8]; // Y - параметры R_C_NRk - ячейки

            double Z_x_R_C_NRk = R * 2.0 * (1.0 + N);
            double Z_y_R_C_NRk = R * 2.0 * (1.0 + N);

            Complex p_R_C_NRk = new Complex(0.0, w);
            Complex temp_R_C_NRk = 1.0 / H + p_R_C_NRk * R * C;
            Complex Y_R_C_NRk = temp_R_C_NRk / ((1.0 + G * temp_R_C_NRk) * ((double)(4 * x * y)) * R);

            Complex tetta_x_R_C_NRk = Complex.Sqrt(Z_x_R_C_NRk * Y_R_C_NRk);
            Complex tetta_y_R_C_NRk = Complex.Sqrt(Z_y_R_C_NRk * Y_R_C_NRk);

            double koeff_x_R_C_NRk = 1.0 / Z_x_R_C_NRk;
            double koeff_y_R_C_NRk = 1.0 / Z_y_R_C_NRk;

            Complex tx_R_C_NRk = tetta_x_R_C_NRk / Complex.Tanh(tetta_x_R_C_NRk);
            Complex sx_R_C_NRk = tetta_x_R_C_NRk / Complex.Sinh(tetta_x_R_C_NRk);
            Complex ty_R_C_NRk = tetta_y_R_C_NRk / Complex.Tanh(tetta_y_R_C_NRk);
            Complex sy_R_C_NRk = tetta_y_R_C_NRk / Complex.Sinh(tetta_y_R_C_NRk);

            tx_R_C_NRk *= koeff_x_R_C_NRk;
            sx_R_C_NRk *= koeff_x_R_C_NRk;
            ty_R_C_NRk *= koeff_y_R_C_NRk;
            sy_R_C_NRk *= koeff_y_R_C_NRk;

            double KxN_R_C_NRk = koeff_x_R_C_NRk * N;
            double KyN_R_C_NRk = koeff_y_R_C_NRk * N;
            double Kx_N_R_C_NRk = koeff_x_R_C_NRk / N;
            double Ky_N_R_C_NRk = koeff_y_R_C_NRk / N;

            Mat_R_C_NRk[0,0] = tx_R_C_NRk + ty_R_C_NRk + KxN_R_C_NRk + KyN_R_C_NRk; Mat_R_C_NRk[0,1] = -sx_R_C_NRk - KxN_R_C_NRk; Mat_R_C_NRk[0,2] = -sy_R_C_NRk - KyN_R_C_NRk; Mat_R_C_NRk[0,3] = 0.0; Mat_R_C_NRk[0,4] = koeff_x_R_C_NRk + koeff_y_R_C_NRk - tx_R_C_NRk - ty_R_C_NRk; Mat_R_C_NRk[0,5] = sx_R_C_NRk - koeff_x_R_C_NRk; Mat_R_C_NRk[0,6] = sy_R_C_NRk - koeff_y_R_C_NRk; Mat_R_C_NRk[0,7] = 0.0;
            Mat_R_C_NRk[1,0] = Mat_R_C_NRk[0,1]; Mat_R_C_NRk[1,1] = Mat_R_C_NRk[0,0]; Mat_R_C_NRk[1,2] = 0.0; Mat_R_C_NRk[1,3] = Mat_R_C_NRk[0,2]; Mat_R_C_NRk[1,4] = Mat_R_C_NRk[0,5]; Mat_R_C_NRk[1,5] = Mat_R_C_NRk[0,4]; Mat_R_C_NRk[1,6] = 0.0; Mat_R_C_NRk[1,7] = Mat_R_C_NRk[0,6];
            Mat_R_C_NRk[2,0] = Mat_R_C_NRk[0,2]; Mat_R_C_NRk[2,1] = 0.0; Mat_R_C_NRk[2,2] = Mat_R_C_NRk[0,0]; Mat_R_C_NRk[2,3] = Mat_R_C_NRk[0,1]; Mat_R_C_NRk[2,4] = Mat_R_C_NRk[0,6]; Mat_R_C_NRk[2,5] = 0.0; Mat_R_C_NRk[2,6] = Mat_R_C_NRk[0,4]; Mat_R_C_NRk[2,7] = Mat_R_C_NRk[0,5];
            Mat_R_C_NRk[3,0] = 0.0; Mat_R_C_NRk[3,1] = Mat_R_C_NRk[0,2]; Mat_R_C_NRk[3,2] = Mat_R_C_NRk[0,1]; Mat_R_C_NRk[3,3] = Mat_R_C_NRk[0,0]; Mat_R_C_NRk[3,4] = 0.0; Mat_R_C_NRk[3,5] = Mat_R_C_NRk[0,6]; Mat_R_C_NRk[3,6] = Mat_R_C_NRk[0,5]; Mat_R_C_NRk[3,7] = Mat_R_C_NRk[0,4];
            Mat_R_C_NRk[4,0] = Mat_R_C_NRk[0,4]; Mat_R_C_NRk[4,1] = Mat_R_C_NRk[0,5]; Mat_R_C_NRk[4,2] = Mat_R_C_NRk[0,6]; Mat_R_C_NRk[4,3] = 0.0; Mat_R_C_NRk[4,4] = tx_R_C_NRk + ty_R_C_NRk + Kx_N_R_C_NRk + Ky_N_R_C_NRk; Mat_R_C_NRk[4,5] = -sx_R_C_NRk - Kx_N_R_C_NRk; Mat_R_C_NRk[4,6] = -sy_R_C_NRk - Ky_N_R_C_NRk; Mat_R_C_NRk[4,7] = 0.0;
            Mat_R_C_NRk[5,0] = Mat_R_C_NRk[0,5]; Mat_R_C_NRk[5,1] = Mat_R_C_NRk[0,4]; Mat_R_C_NRk[5,2] = 0.0; Mat_R_C_NRk[5,3] = Mat_R_C_NRk[0,6]; Mat_R_C_NRk[5,4] = Mat_R_C_NRk[4,5]; Mat_R_C_NRk[5,5] = Mat_R_C_NRk[4,4]; Mat_R_C_NRk[5,6] = 0.0; Mat_R_C_NRk[5,7] = Mat_R_C_NRk[4,6];
            Mat_R_C_NRk[6,0] = Mat_R_C_NRk[0,6]; Mat_R_C_NRk[6,1] = 0.0; Mat_R_C_NRk[6,2] = Mat_R_C_NRk[0,4]; Mat_R_C_NRk[6,3] = Mat_R_C_NRk[0,5]; Mat_R_C_NRk[6,4] = Mat_R_C_NRk[4,6]; Mat_R_C_NRk[6,5] = 0.0; Mat_R_C_NRk[6,6] = Mat_R_C_NRk[4,4]; Mat_R_C_NRk[6,7] = Mat_R_C_NRk[4,5];
            Mat_R_C_NRk[7,0] = 0.0;

            #endregion

            #region RCNR

            Complex[,] Mat_rcnr = new Complex[8,8]; // Y - параметры RCNR - ячейки

            double Z_x = R * 2.0 * (1.0 + N);
            double Z_y = R * 2.0 * (1.0 + N);

            Complex p = new Complex(0.0, w);
            Complex temp = 1.0 / H + p * R * C;
            Complex Y = temp / ((1.0 + G * temp) * ((double)(4 * x * y)) * R);

            Complex tetta_x = Complex.Sqrt(Z_x * Y);
            Complex tetta_y = Complex.Sqrt(Z_y * Y);

            double koeff_x = 1.0 / Z_x;
            double koeff_y = 1.0 / Z_y;

            Complex tx = tetta_x / Complex.Tanh(tetta_x);
            Complex sx = tetta_x / Complex.Sinh(tetta_x);
            Complex ty = tetta_y / Complex.Tanh(tetta_y);
            Complex sy = tetta_y / Complex.Sinh(tetta_y);

            tx *= koeff_x;
            sx *= koeff_x;
            ty *= koeff_y;
            sy *= koeff_y;

            double KxN = koeff_x * N;
            double KyN = koeff_y * N;
            double Kx_N = koeff_x / N;
            double Ky_N = koeff_y / N;

            Mat_rcnr[0,0] = tx + ty + KxN + KyN; Mat_rcnr[0,1] = -sx - KxN; Mat_rcnr[0,2] = -sy - KyN; Mat_rcnr[0,3] = 0.0; Mat_rcnr[0,4] = koeff_x + koeff_y - tx - ty; Mat_rcnr[0,5] = sx - koeff_x; Mat_rcnr[0,6] = sy - koeff_y; Mat_rcnr[0,7] = 0.0;
            Mat_rcnr[1,0] = Mat_rcnr[0,1]; Mat_rcnr[1,1] = Mat_rcnr[0,0]; Mat_rcnr[1,2] = 0.0; Mat_rcnr[1,3] = Mat_rcnr[0,2]; Mat_rcnr[1,4] = Mat_rcnr[0,5]; Mat_rcnr[1,5] = Mat_rcnr[0,4]; Mat_rcnr[1,6] = 0.0; Mat_rcnr[1,7] = Mat_rcnr[0,6];
            Mat_rcnr[2,0] = Mat_rcnr[0,2]; Mat_rcnr[2,1] = 0.0; Mat_rcnr[2,2] = Mat_rcnr[0,0]; Mat_rcnr[2,3] = Mat_rcnr[0,1]; Mat_rcnr[2,4] = Mat_rcnr[0,6]; Mat_rcnr[2,5] = 0.0; Mat_rcnr[2,6] = Mat_rcnr[0,4]; Mat_rcnr[2,7] = Mat_rcnr[0,5];
            Mat_rcnr[3,0] = 0.0; Mat_rcnr[3,1] = Mat_rcnr[0,2]; Mat_rcnr[3,2] = Mat_rcnr[0,1]; Mat_rcnr[3,3] = Mat_rcnr[0,0]; Mat_rcnr[3,4] = 0.0; Mat_rcnr[3,5] = Mat_rcnr[0,6]; Mat_rcnr[3,6] = Mat_rcnr[0,5]; Mat_rcnr[3,7] = Mat_rcnr[0,4];
            Mat_rcnr[4,0] = Mat_rcnr[0,4]; Mat_rcnr[4,1] = Mat_rcnr[0,5]; Mat_rcnr[4,2] = Mat_rcnr[0,6]; Mat_rcnr[4,3] = 0.0; Mat_rcnr[4,4] = tx + ty + Kx_N + Ky_N; Mat_rcnr[4,5] = -sx - Kx_N; Mat_rcnr[4,6] = -sy - Ky_N; Mat_rcnr[4,7] = 0.0;
            Mat_rcnr[5,0] = Mat_rcnr[0,5]; Mat_rcnr[5,1] = Mat_rcnr[0,4]; Mat_rcnr[5,2] = 0.0; Mat_rcnr[5,3] = Mat_rcnr[0,6]; Mat_rcnr[5,4] = Mat_rcnr[4,5]; Mat_rcnr[5,5] = Mat_rcnr[4,4]; Mat_rcnr[5,6] = 0.0; Mat_rcnr[5,7] = Mat_rcnr[4,6];
            Mat_rcnr[6,0] = Mat_rcnr[0,6]; Mat_rcnr[6,1] = 0.0; Mat_rcnr[6,2] = Mat_rcnr[0,4]; Mat_rcnr[6,3] = Mat_rcnr[0,5]; Mat_rcnr[6,4] = Mat_rcnr[4,6]; Mat_rcnr[6,5] = 0.0; Mat_rcnr[6,6] = Mat_rcnr[4,4]; Mat_rcnr[6,7] = Mat_rcnr[4,5];
            Mat_rcnr[7,0] = 0.0; Mat_rcnr[7,1] = Mat_rcnr[0,6]; Mat_rcnr[7,2] = Mat_rcnr[0,5]; Mat_rcnr[7,3] = Mat_rcnr[0,4]; Mat_rcnr[7,4] = 0.0; Mat_rcnr[7,5] = Mat_rcnr[4,6]; Mat_rcnr[7,6] = Mat_rcnr[4,5]; Mat_rcnr[7,7] = Mat_rcnr[4,4];

            #endregion

            // глобальная матрица y-параметров
            GlobalY = Matrix<Complex>.Build.DenseOfArray(new Complex[nodesCount, nodesCount]);

            for (int i = 0; i < x; i++)// горизонтальная ось
            {
                for (int j = 0; j < y; j++)// вертикальная ось
                {
                    switch (structure.Segments[j][i].SegmentType)
                    {
                        case StructureSegmentTypeEnum.EMPTY:
                            break;
                        case StructureSegmentTypeEnum.R_C_NR:
                            {
                                var Element = Mat_rcnr;

                                int u1 = nodesNumeration[0, i, j];
                                int u2 = nodesNumeration[0, i + 1, j];
                                int u3 = nodesNumeration[0, i, j + 1];
                                int u4 = nodesNumeration[0, i + 1, j + 1];
                                int u5 = nodesNumeration[1, i, j];
                                int u6 = nodesNumeration[1, i + 1, j];
                                int u7 = nodesNumeration[1, i, j + 1];
                                int u8 = nodesNumeration[1, i + 1, j + 1];

                                AddNodeToGlobalMatrix(u1, u1, Element[0, 0], false);
                                AddNodeToGlobalMatrix(u1, u2, Element[0, 1], true);
                                AddNodeToGlobalMatrix(u1, u3, Element[0, 2], true);
                                AddNodeToGlobalMatrix(u1, u4, Element[0, 3], true);
                                AddNodeToGlobalMatrix(u1, u5, Element[0, 4], true);
                                AddNodeToGlobalMatrix(u1, u6, Element[0, 5], true);
                                AddNodeToGlobalMatrix(u1, u7, Element[0, 6], true);
                                AddNodeToGlobalMatrix(u1, u8, Element[0, 7], true);

                                AddNodeToGlobalMatrix(u2, u2, Element[1, 1], false);
                                AddNodeToGlobalMatrix(u2, u3, Element[1, 2], true);
                                AddNodeToGlobalMatrix(u2, u4, Element[1, 3], true);
                                AddNodeToGlobalMatrix(u2, u5, Element[1, 4], true);
                                AddNodeToGlobalMatrix(u2, u6, Element[1, 5], true);
                                AddNodeToGlobalMatrix(u2, u7, Element[1, 6], true);
                                AddNodeToGlobalMatrix(u2, u8, Element[1, 7], true);

                                AddNodeToGlobalMatrix(u3, u3, Element[2, 2], false);
                                AddNodeToGlobalMatrix(u3, u4, Element[2, 3], true);
                                AddNodeToGlobalMatrix(u3, u5, Element[2, 4], true);
                                AddNodeToGlobalMatrix(u3, u6, Element[2, 5], true);
                                AddNodeToGlobalMatrix(u3, u7, Element[2, 6], true);
                                AddNodeToGlobalMatrix(u3, u8, Element[2, 7], true);

                                AddNodeToGlobalMatrix(u4, u4, Element[3, 3], false);
                                AddNodeToGlobalMatrix(u4, u5, Element[3, 4], true);
                                AddNodeToGlobalMatrix(u4, u6, Element[3, 5], true);
                                AddNodeToGlobalMatrix(u4, u7, Element[3, 6], true);
                                AddNodeToGlobalMatrix(u4, u8, Element[3, 7], true);

                                AddNodeToGlobalMatrix(u5, u5, Element[4, 4], false);
                                AddNodeToGlobalMatrix(u5, u6, Element[4, 5], true);
                                AddNodeToGlobalMatrix(u5, u7, Element[4, 6], true);
                                AddNodeToGlobalMatrix(u5, u8, Element[4, 7], true);

                                AddNodeToGlobalMatrix(u6, u6, Element[5, 5], false);
                                AddNodeToGlobalMatrix(u6, u7, Element[5, 6], true);
                                AddNodeToGlobalMatrix(u6, u8, Element[5, 7], true);

                                AddNodeToGlobalMatrix(u7, u7, Element[6, 6], false);
                                AddNodeToGlobalMatrix(u7, u8, Element[6, 7], true);

                                AddNodeToGlobalMatrix(u8, u8, Element[7, 7], false);
                            }
                            break;
                        case StructureSegmentTypeEnum.Rv:
                            {
                                var Element = Mat_r;

                                int u1 = nodesNumeration[0, i, j];
                                int u2 = nodesNumeration[0, i + 1, j];
                                int u3 = nodesNumeration[0, i, j + 1];
                                int u4 = nodesNumeration[0, i + 1, j + 1];
                                int u5 = nodesNumeration[1, i, j];
                                int u6 = nodesNumeration[1, i + 1, j];
                                int u7 = nodesNumeration[1, i, j + 1];
                                int u8 = nodesNumeration[1, i + 1, j + 1];

                                AddNodeToGlobalMatrix(u1, u1, Element[0, 0], false);
                                AddNodeToGlobalMatrix(u1, u2, Element[0, 1], true);
                                AddNodeToGlobalMatrix(u1, u3, Element[0, 2], true);
                                AddNodeToGlobalMatrix(u1, u4, Element[0, 3], true);
                                AddNodeToGlobalMatrix(u1, u5, Element[0, 4], true);
                                AddNodeToGlobalMatrix(u1, u6, Element[0, 5], true);
                                AddNodeToGlobalMatrix(u1, u7, Element[0, 6], true);
                                AddNodeToGlobalMatrix(u1, u8, Element[0, 7], true);

                                AddNodeToGlobalMatrix(u2, u2, Element[1, 1], false);
                                AddNodeToGlobalMatrix(u2, u3, Element[1, 2], true);
                                AddNodeToGlobalMatrix(u2, u4, Element[1, 3], true);
                                AddNodeToGlobalMatrix(u2, u5, Element[1, 4], true);
                                AddNodeToGlobalMatrix(u2, u6, Element[1, 5], true);
                                AddNodeToGlobalMatrix(u2, u7, Element[1, 6], true);
                                AddNodeToGlobalMatrix(u2, u8, Element[1, 7], true);

                                AddNodeToGlobalMatrix(u3, u3, Element[2, 2], false);
                                AddNodeToGlobalMatrix(u3, u4, Element[2, 3], true);
                                AddNodeToGlobalMatrix(u3, u5, Element[2, 4], true);
                                AddNodeToGlobalMatrix(u3, u6, Element[2, 5], true);
                                AddNodeToGlobalMatrix(u3, u7, Element[2, 6], true);
                                AddNodeToGlobalMatrix(u3, u8, Element[2, 7], true);

                                AddNodeToGlobalMatrix(u4, u4, Element[3, 3], false);
                                AddNodeToGlobalMatrix(u4, u5, Element[3, 4], true);
                                AddNodeToGlobalMatrix(u4, u6, Element[3, 5], true);
                                AddNodeToGlobalMatrix(u4, u7, Element[3, 6], true);
                                AddNodeToGlobalMatrix(u4, u8, Element[3, 7], true);

                                AddNodeToGlobalMatrix(u5, u5, Element[4, 4], false);
                                AddNodeToGlobalMatrix(u5, u6, Element[4, 5], true);
                                AddNodeToGlobalMatrix(u5, u7, Element[4, 6], true);
                                AddNodeToGlobalMatrix(u5, u8, Element[4, 7], true);

                                AddNodeToGlobalMatrix(u6, u6, Element[5, 5], false);
                                AddNodeToGlobalMatrix(u6, u7, Element[5, 6], true);
                                AddNodeToGlobalMatrix(u6, u8, Element[5, 7], true);

                                AddNodeToGlobalMatrix(u7, u7, Element[6, 6], false);
                                AddNodeToGlobalMatrix(u7, u8, Element[6, 7], true);

                                AddNodeToGlobalMatrix(u8, u8, Element[7, 7], false);
                            }
                            break;
                        case StructureSegmentTypeEnum.Rn:
                            {
                                var Element = Mat_nr;

                                int u1 = nodesNumeration[0, i, j];
                                int u2 = nodesNumeration[0, i + 1, j];
                                int u3 = nodesNumeration[0, i, j + 1];
                                int u4 = nodesNumeration[0, i + 1, j + 1];
                                int u5 = nodesNumeration[1, i, j];
                                int u6 = nodesNumeration[1, i + 1, j];
                                int u7 = nodesNumeration[1, i, j + 1];
                                int u8 = nodesNumeration[1, i + 1, j + 1];

                                AddNodeToGlobalMatrix(u1, u1, Element[0, 0], false);
                                AddNodeToGlobalMatrix(u1, u2, Element[0, 1], true);
                                AddNodeToGlobalMatrix(u1, u3, Element[0, 2], true);
                                AddNodeToGlobalMatrix(u1, u4, Element[0, 3], true);
                                AddNodeToGlobalMatrix(u1, u5, Element[0, 4], true);
                                AddNodeToGlobalMatrix(u1, u6, Element[0, 5], true);
                                AddNodeToGlobalMatrix(u1, u7, Element[0, 6], true);
                                AddNodeToGlobalMatrix(u1, u8, Element[0, 7], true);

                                AddNodeToGlobalMatrix(u2, u2, Element[1, 1], false);
                                AddNodeToGlobalMatrix(u2, u3, Element[1, 2], true);
                                AddNodeToGlobalMatrix(u2, u4, Element[1, 3], true);
                                AddNodeToGlobalMatrix(u2, u5, Element[1, 4], true);
                                AddNodeToGlobalMatrix(u2, u6, Element[1, 5], true);
                                AddNodeToGlobalMatrix(u2, u7, Element[1, 6], true);
                                AddNodeToGlobalMatrix(u2, u8, Element[1, 7], true);

                                AddNodeToGlobalMatrix(u3, u3, Element[2, 2], false);
                                AddNodeToGlobalMatrix(u3, u4, Element[2, 3], true);
                                AddNodeToGlobalMatrix(u3, u5, Element[2, 4], true);
                                AddNodeToGlobalMatrix(u3, u6, Element[2, 5], true);
                                AddNodeToGlobalMatrix(u3, u7, Element[2, 6], true);
                                AddNodeToGlobalMatrix(u3, u8, Element[2, 7], true);

                                AddNodeToGlobalMatrix(u4, u4, Element[3, 3], false);
                                AddNodeToGlobalMatrix(u4, u5, Element[3, 4], true);
                                AddNodeToGlobalMatrix(u4, u6, Element[3, 5], true);
                                AddNodeToGlobalMatrix(u4, u7, Element[3, 6], true);
                                AddNodeToGlobalMatrix(u4, u8, Element[3, 7], true);

                                AddNodeToGlobalMatrix(u5, u5, Element[4, 4], false);
                                AddNodeToGlobalMatrix(u5, u6, Element[4, 5], true);
                                AddNodeToGlobalMatrix(u5, u7, Element[4, 6], true);
                                AddNodeToGlobalMatrix(u5, u8, Element[4, 7], true);

                                AddNodeToGlobalMatrix(u6, u6, Element[5, 5], false);
                                AddNodeToGlobalMatrix(u6, u7, Element[5, 6], true);
                                AddNodeToGlobalMatrix(u6, u8, Element[5, 7], true);

                                AddNodeToGlobalMatrix(u7, u7, Element[6, 6], false);
                                AddNodeToGlobalMatrix(u7, u8, Element[6, 7], true);

                                AddNodeToGlobalMatrix(u8, u8, Element[7, 7], false);
                            }
                            break;
                        case StructureSegmentTypeEnum.Rk_C_NRk:
                            {
                                var Element = Mat_Rk_C_NRk;

                                int u1 = nodesNumeration[0, i, j];
                                int u2 = nodesNumeration[0, i + 1, j];
                                int u3 = nodesNumeration[0, i, j + 1];
                                int u4 = nodesNumeration[0, i + 1, j + 1];
                                int u5 = nodesNumeration[1, i, j];
                                int u6 = nodesNumeration[1, i + 1, j];
                                int u7 = nodesNumeration[1, i, j + 1];
                                int u8 = nodesNumeration[1, i + 1, j + 1];

                                AddNodeToGlobalMatrix(u1, u1, Element[0, 0], false);
                                AddNodeToGlobalMatrix(u1, u2, Element[0, 1], true);
                                AddNodeToGlobalMatrix(u1, u3, Element[0, 2], true);
                                AddNodeToGlobalMatrix(u1, u4, Element[0, 3], true);
                                AddNodeToGlobalMatrix(u1, u5, Element[0, 4], true);
                                AddNodeToGlobalMatrix(u1, u6, Element[0, 5], true);
                                AddNodeToGlobalMatrix(u1, u7, Element[0, 6], true);
                                AddNodeToGlobalMatrix(u1, u8, Element[0, 7], true);

                                AddNodeToGlobalMatrix(u2, u2, Element[1, 1], false);
                                AddNodeToGlobalMatrix(u2, u3, Element[1, 2], true);
                                AddNodeToGlobalMatrix(u2, u4, Element[1, 3], true);
                                AddNodeToGlobalMatrix(u2, u5, Element[1, 4], true);
                                AddNodeToGlobalMatrix(u2, u6, Element[1, 5], true);
                                AddNodeToGlobalMatrix(u2, u7, Element[1, 6], true);
                                AddNodeToGlobalMatrix(u2, u8, Element[1, 7], true);

                                AddNodeToGlobalMatrix(u3, u3, Element[2, 2], false);
                                AddNodeToGlobalMatrix(u3, u4, Element[2, 3], true);
                                AddNodeToGlobalMatrix(u3, u5, Element[2, 4], true);
                                AddNodeToGlobalMatrix(u3, u6, Element[2, 5], true);
                                AddNodeToGlobalMatrix(u3, u7, Element[2, 6], true);
                                AddNodeToGlobalMatrix(u3, u8, Element[2, 7], true);

                                AddNodeToGlobalMatrix(u4, u4, Element[3, 3], false);
                                AddNodeToGlobalMatrix(u4, u5, Element[3, 4], true);
                                AddNodeToGlobalMatrix(u4, u6, Element[3, 5], true);
                                AddNodeToGlobalMatrix(u4, u7, Element[3, 6], true);
                                AddNodeToGlobalMatrix(u4, u8, Element[3, 7], true);

                                AddNodeToGlobalMatrix(u5, u5, Element[4, 4], false);
                                AddNodeToGlobalMatrix(u5, u6, Element[4, 5], true);
                                AddNodeToGlobalMatrix(u5, u7, Element[4, 6], true);
                                AddNodeToGlobalMatrix(u5, u8, Element[4, 7], true);

                                AddNodeToGlobalMatrix(u6, u6, Element[5, 5], false);
                                AddNodeToGlobalMatrix(u6, u7, Element[5, 6], true);
                                AddNodeToGlobalMatrix(u6, u8, Element[5, 7], true);

                                AddNodeToGlobalMatrix(u7, u7, Element[6, 6], false);
                                AddNodeToGlobalMatrix(u7, u8, Element[6, 7], true);

                                AddNodeToGlobalMatrix(u8, u8, Element[7, 7], false);
                            }
                            break;
                        case StructureSegmentTypeEnum.R_C_NRk:
                            {
                                var Element = Mat_R_C_NRk;

                                int u1 = nodesNumeration[0, i, j];
                                int u2 = nodesNumeration[0, i + 1, j];
                                int u3 = nodesNumeration[0, i, j + 1];
                                int u4 = nodesNumeration[0, i + 1, j + 1];
                                int u5 = nodesNumeration[1, i, j];
                                int u6 = nodesNumeration[1, i + 1, j];
                                int u7 = nodesNumeration[1, i, j + 1];
                                int u8 = nodesNumeration[1, i + 1, j + 1];

                                AddNodeToGlobalMatrix(u1, u1, Element[0, 0], false);
                                AddNodeToGlobalMatrix(u1, u2, Element[0, 1], true);
                                AddNodeToGlobalMatrix(u1, u3, Element[0, 2], true);
                                AddNodeToGlobalMatrix(u1, u4, Element[0, 3], true);
                                AddNodeToGlobalMatrix(u1, u5, Element[0, 4], true);
                                AddNodeToGlobalMatrix(u1, u6, Element[0, 5], true);
                                AddNodeToGlobalMatrix(u1, u7, Element[0, 6], true);
                                AddNodeToGlobalMatrix(u1, u8, Element[0, 7], true);

                                AddNodeToGlobalMatrix(u2, u2, Element[1, 1], false);
                                AddNodeToGlobalMatrix(u2, u3, Element[1, 2], true);
                                AddNodeToGlobalMatrix(u2, u4, Element[1, 3], true);
                                AddNodeToGlobalMatrix(u2, u5, Element[1, 4], true);
                                AddNodeToGlobalMatrix(u2, u6, Element[1, 5], true);
                                AddNodeToGlobalMatrix(u2, u7, Element[1, 6], true);
                                AddNodeToGlobalMatrix(u2, u8, Element[1, 7], true);

                                AddNodeToGlobalMatrix(u3, u3, Element[2, 2], false);
                                AddNodeToGlobalMatrix(u3, u4, Element[2, 3], true);
                                AddNodeToGlobalMatrix(u3, u5, Element[2, 4], true);
                                AddNodeToGlobalMatrix(u3, u6, Element[2, 5], true);
                                AddNodeToGlobalMatrix(u3, u7, Element[2, 6], true);
                                AddNodeToGlobalMatrix(u3, u8, Element[2, 7], true);

                                AddNodeToGlobalMatrix(u4, u4, Element[3, 3], false);
                                AddNodeToGlobalMatrix(u4, u5, Element[3, 4], true);
                                AddNodeToGlobalMatrix(u4, u6, Element[3, 5], true);
                                AddNodeToGlobalMatrix(u4, u7, Element[3, 6], true);
                                AddNodeToGlobalMatrix(u4, u8, Element[3, 7], true);

                                AddNodeToGlobalMatrix(u5, u5, Element[4, 4], false);
                                AddNodeToGlobalMatrix(u5, u6, Element[4, 5], true);
                                AddNodeToGlobalMatrix(u5, u7, Element[4, 6], true);
                                AddNodeToGlobalMatrix(u5, u8, Element[4, 7], true);

                                AddNodeToGlobalMatrix(u6, u6, Element[5, 5], false);
                                AddNodeToGlobalMatrix(u6, u7, Element[5, 6], true);
                                AddNodeToGlobalMatrix(u6, u8, Element[5, 7], true);

                                AddNodeToGlobalMatrix(u7, u7, Element[6, 6], false);
                                AddNodeToGlobalMatrix(u7, u8, Element[6, 7], true);

                                AddNodeToGlobalMatrix(u8, u8, Element[7, 7], false);
                            }
                            break;
                        case StructureSegmentTypeEnum.Rk_C_NR:
                            {
                                var Element = Mat_Rk_C_NR;

                                int u1 = nodesNumeration[0, i, j];
                                int u2 = nodesNumeration[0, i + 1, j];
                                int u3 = nodesNumeration[0, i, j + 1];
                                int u4 = nodesNumeration[0, i + 1, j + 1];
                                int u5 = nodesNumeration[1, i, j];
                                int u6 = nodesNumeration[1, i + 1, j];
                                int u7 = nodesNumeration[1, i, j + 1];
                                int u8 = nodesNumeration[1, i + 1, j + 1];

                                AddNodeToGlobalMatrix(u1, u1, Element[0, 0], false);
                                AddNodeToGlobalMatrix(u1, u2, Element[0, 1], true);
                                AddNodeToGlobalMatrix(u1, u3, Element[0, 2], true);
                                AddNodeToGlobalMatrix(u1, u4, Element[0, 3], true);
                                AddNodeToGlobalMatrix(u1, u5, Element[0, 4], true);
                                AddNodeToGlobalMatrix(u1, u6, Element[0, 5], true);
                                AddNodeToGlobalMatrix(u1, u7, Element[0, 6], true);
                                AddNodeToGlobalMatrix(u1, u8, Element[0, 7], true);

                                AddNodeToGlobalMatrix(u2, u2, Element[1, 1], false);
                                AddNodeToGlobalMatrix(u2, u3, Element[1, 2], true);
                                AddNodeToGlobalMatrix(u2, u4, Element[1, 3], true);
                                AddNodeToGlobalMatrix(u2, u5, Element[1, 4], true);
                                AddNodeToGlobalMatrix(u2, u6, Element[1, 5], true);
                                AddNodeToGlobalMatrix(u2, u7, Element[1, 6], true);
                                AddNodeToGlobalMatrix(u2, u8, Element[1, 7], true);

                                AddNodeToGlobalMatrix(u3, u3, Element[2, 2], false);
                                AddNodeToGlobalMatrix(u3, u4, Element[2, 3], true);
                                AddNodeToGlobalMatrix(u3, u5, Element[2, 4], true);
                                AddNodeToGlobalMatrix(u3, u6, Element[2, 5], true);
                                AddNodeToGlobalMatrix(u3, u7, Element[2, 6], true);
                                AddNodeToGlobalMatrix(u3, u8, Element[2, 7], true);

                                AddNodeToGlobalMatrix(u4, u4, Element[3, 3], false);
                                AddNodeToGlobalMatrix(u4, u5, Element[3, 4], true);
                                AddNodeToGlobalMatrix(u4, u6, Element[3, 5], true);
                                AddNodeToGlobalMatrix(u4, u7, Element[3, 6], true);
                                AddNodeToGlobalMatrix(u4, u8, Element[3, 7], true);

                                AddNodeToGlobalMatrix(u5, u5, Element[4, 4], false);
                                AddNodeToGlobalMatrix(u5, u6, Element[4, 5], true);
                                AddNodeToGlobalMatrix(u5, u7, Element[4, 6], true);
                                AddNodeToGlobalMatrix(u5, u8, Element[4, 7], true);

                                AddNodeToGlobalMatrix(u6, u6, Element[5, 5], false);
                                AddNodeToGlobalMatrix(u6, u7, Element[5, 6], true);
                                AddNodeToGlobalMatrix(u6, u8, Element[5, 7], true);

                                AddNodeToGlobalMatrix(u7, u7, Element[6, 6], false);
                                AddNodeToGlobalMatrix(u7, u8, Element[6, 7], true);

                                AddNodeToGlobalMatrix(u8, u8, Element[7, 7], false);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // Метод для добавления проводимости узла к глобальной матрице проводимостей
        private void AddNodeToGlobalMatrix(int u0, int u1, Complex value, bool f) 
        {
            if (u0 >= 0 && u1 >= 0 && value != 0.0)
            {
                if (u1 > u0)
                {
                    GlobalY[u0, u1] += value;
                    GlobalY[u1, u0] += value;
                }
                else
                {
                    GlobalY[u1, u0] += value;
                    GlobalY[u0, u1] += value;

                    if (f && u1 == u0)  // когда номера узлов u1 и u0 одинаковы, но эти узлы не относятся к собственному узлу,
                    {
                        GlobalY[u1, 0] += value; // то проводимость между этими узлами надо прибавлять дважды
                        GlobalY[0, u1] += value;
                    }
                }
            } 
        }

        // Метод для поиска номеров заземлённых и соединённых выводов
        private (List<int> PE, Matrix<float> I) FindPEPinsAndConnectedPinsIndices(RCStructureBase structure)
        {
            // номера заземлённых выводов
            var peVector = new List<int>();

            // матрица соединений (инцидентности)
            var I = Matrix<float>.Build.DenseOfArray(new float[NodesCount, NodesCount]);

            for (int i = 0; i < HorizontalDimension; i++)// горизонтальная ось
            {
                for (int j = 0; j < VerticalDimension; j++)// вертикальная ось
                {
                    switch (structure.Segments[j][i].SegmentType)
                    {
                        case StructureSegmentTypeEnum.EMPTY:
                            break;
                        case StructureSegmentTypeEnum.R_C_NR:
                            break;
                        case StructureSegmentTypeEnum.Rv:
                            break;
                        case StructureSegmentTypeEnum.Rn:
                            break;
                        case StructureSegmentTypeEnum.Rk_C_NRk:
                            {
                                int u1 = NodesNumeration[0, i, j];
                                int u2 = NodesNumeration[0, i + 1, j];
                                int u3 = NodesNumeration[0, i, j + 1];
                                int u4 = NodesNumeration[0, i + 1, j + 1];
                                int u5 = NodesNumeration[1, i, j];
                                int u6 = NodesNumeration[1, i + 1, j];
                                int u7 = NodesNumeration[1, i, j + 1];
                                int u8 = NodesNumeration[1, i + 1, j + 1];

                                // меньший номер идёт первым индексом , так как заполняется верхний треугольник матрицы
                                if (u1 > u5)
                                {
                                    I[u5, u1] = 1;
                                }
                                else
                                {
                                    I[u1, u5] = 1;
                                }

                                if (u2 > u6)
                                {
                                    I[u6, u2] = 1;
                                }
                                else
                                {
                                    I[u2, u6] = 1;
                                }

                                if (u3 > u7)
                                {
                                    I[u7, u3] = 1;
                                }
                                else
                                {
                                    I[u3, u7] = 1;
                                }

                                if (u4 > u8)
                                {
                                    I[u8, u4] = 1;
                                }
                                else
                                {
                                    I[u4, u8] = 1;
                                }
                            }
                            break;
                        case StructureSegmentTypeEnum.R_C_NRk:
                            {
                                int u5 = NodesNumeration[1, i, j];
                                int u6 = NodesNumeration[1, i + 1, j];
                                int u7 = NodesNumeration[1, i, j + 1];
                                int u8 = NodesNumeration[1, i + 1, j + 1];

                                peVector.Add(u5);
                                peVector.Add(u6);
                                peVector.Add(u7);
                                peVector.Add(u8);
                            }
                            break;
                        case StructureSegmentTypeEnum.Rk_C_NR:
                            {
                                int u1 = NodesNumeration[0, i, j];
                                int u2 = NodesNumeration[0, i + 1, j];
                                int u3 = NodesNumeration[0, i, j + 1];
                                int u4 = NodesNumeration[0, i + 1, j + 1];

                                peVector.Add(u1);
                                peVector.Add(u2);
                                peVector.Add(u3);
                                peVector.Add(u4);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            peVector = peVector.Distinct().OrderBy(x => x).ToList();

            return (peVector, I);
        }

        // Метод для расчёта фазы
        public double CalculatePhase(double frequency)
        {
            //frequency = frequency * 2.0 * System.Math.PI;

            FillGlobalMatrix(Structure, NodesCount, NodesNumeration, frequency);

            // удалить строки и столбцы
            SchemePhaseResponseCalculator.RemoveRowAndColsFromMatrix(ref GlobalY, PEPinsAndConnectedPinsIndices.PE);
            // сложить строки и столбцы
            SchemePhaseResponseCalculator.AddRowsAndColsInYMatrix(ref GlobalY, ref PEPinsAndConnectedPinsIndices.I);
            // понизить порядок глобальной матрицы до числа внешних выводов
            SchemePhaseResponseCalculator.ReduceMatrix(ref GlobalY, Structure.Scheme.Model.OuterPins.Count);

            SchemePhaseResponseCalculator.RemoveRowAndColsFromMatrix(ref GlobalY, GroundedOuterPins);
            SchemePhaseResponseCalculator.RemoveRowAndColsFromMatrix(ref ConnectedOuterPinsMatrix, GroundedOuterPins);

            SchemePhaseResponseCalculator.AddRowsAndColsInYMatrix(ref GlobalY, ref ConnectedOuterPinsMatrix);

            SchemePhaseResponseCalculator.ReduceMatrix(ref GlobalY, 1);

            var phase = -GlobalY[GlobalY.RowCount - 1, GlobalY.ColumnCount - 1].Phase * 180 / Math.PI;

            return phase;
        }
    }
}
