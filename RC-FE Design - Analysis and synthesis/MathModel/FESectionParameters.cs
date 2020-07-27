using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Параметры секции
    /// </summary>
    public class FESectionParameters
    {
        /// <summary>
        /// Тип секции
        /// </summary>
        public FESectionTypeEnum SectionType { get; set; } = FESectionTypeEnum.R_C_NR;

        public double C { get; set; }

        public double R { get; set; }

        public double N { get; set; }

        public double L { get; set; }

        /// <summary>
        /// Технологический параметр G
        /// </summary>
        public double G { get; set; }

        /// <summary>
        /// Технологический параметр Rp  
        /// </summary>
        public double Rp { get; set; }

        /// <summary>
        /// Технологический параметр Rk
        /// </summary>
        public double Rk { get; set; }
    }
}
