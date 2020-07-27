using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Класс представляет базовый конструктивный элемент
    /// </summary>
    public class FESection
    {
        /// <summary>
        /// Словарь функций для расчета 
        /// </summary>
        public static Dictionary<FESectionTypeEnum, Func<double, double, double, double, double, double, double, double, Complex>> ThetaFunctions = 
            new Dictionary<FESectionTypeEnum, Func<double, double, double, double, double, double, double, double, Complex>>() 
        {
            { 
                FESectionTypeEnum.R_C_NR, 
                (double R, double N, double C, double Rp, double Rk, double G, double L, double w) => 
                {
                    double dela = 1;
                    double delb = 100_000_000;

                    w = 2 * Math.PI * w;

                    var compl_f = new Complex(1, w*C*delb*Math.Pow(L,2));

                    Complex theta = Complex.Sqrt( (R*(1+N) * compl_f) / (delb + dela * compl_f) );

                    return theta;
                } 
            },
            {
                FESectionTypeEnum.O_R_C_NR_O,
                (double R, double N, double C, double Rp, double Rk, double G, double L, double w) =>
                {
                    Complex theta = 0;

                    throw new NotImplementedException();

                    return theta;
                }
            },
            {
                FESectionTypeEnum.O_R_C_NR,
                (double R, double N, double C, double Rp, double Rk, double G, double L, double w) =>
                {
                    Complex theta = 0;

                    throw new NotImplementedException();

                    return theta;
                }
            },
            {
                FESectionTypeEnum.R_C_NR_O,
                (double R, double N, double C, double Rp, double Rk, double G, double L, double w) =>
                {
                    Complex theta = 0;

                    throw new NotImplementedException();

                    return theta;
                }
            }

        };

        public FESection(FESectionParameters sectionParameters, List<Pin> pins, int number)
        {
            Number = number;
            Pins = pins;

            foreach (var pin in pins)
                pin.Section = this;

            SectionParameters = sectionParameters;
            YParametersMatrix = Matrix<Complex>.Build.DenseOfArray(new Complex[pins.Count, pins.Count]);
        }

        /// <summary>
        /// Номер секции
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Список выводов секции
        /// </summary>
        public List<Pin> Pins { get; set; }

        /// <summary>
        /// Параметры секции
        /// </summary>
        public FESectionParameters SectionParameters { get; set; }

        /// <summary>
        /// Матрица проводимости секции
        /// </summary>
        public Matrix<Complex> YParametersMatrix { get; set; }
    }
}
