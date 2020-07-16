using System;
using System.Collections.Generic;
using System.Linq;
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
        public static Dictionary<FESectionTypeEnum, Func<double, double, double, double, double, double, double, double, double>> ThetaFunctions = 
            new Dictionary<FESectionTypeEnum, Func<double, double, double, double, double, double, double, double, double>>() 
        {
            { 
                FESectionTypeEnum.R_C_NR, 
                (double R, double N, double C, double Rp, double Rk, double G, double L, double w) => 
                {
                    double theta = 0;// L*Math.Sqrt((R*(1+N)*(1+))/());



                    return theta;
                } 
            },
            {
                FESectionTypeEnum.O_R_C_NR_O,
                (double R, double N, double C, double Rp, double Rk, double G, double L, double w) =>
                {
                    double theta = 0;



                    return theta;
                }
            },
            {
                FESectionTypeEnum.O_R_C_NR,
                (double R, double N, double C, double Rp, double Rk, double G, double L, double w) =>
                {
                    double theta = 0;



                    return theta;
                }
            },
            {
                FESectionTypeEnum.R_C_NR_O,
                (double R, double N, double C, double Rp, double Rk, double G, double L, double w) =>
                {
                    double theta = 0;



                    return theta;
                }
            }

        };

        public FESection(FESectionParameters sectionParameters)
        {
            SectionParameters = sectionParameters;
        }

        public FESectionParameters SectionParameters { get; set; }

        public int[] SchemeIndices { get; set; } = { 0, 0 }; 
    }
}
