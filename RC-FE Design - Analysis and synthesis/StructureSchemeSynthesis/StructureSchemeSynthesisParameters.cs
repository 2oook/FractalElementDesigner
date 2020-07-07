using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.StructureSchemeSynthesis
{
    /// <summary>
    /// Класс параметров синтеза схемы структуры
    /// </summary>
    public class StructureSchemeSynthesisParameters : IDataErrorInfo
    {
        /// <summary>
        /// Количество итераций 1-го такта ГА
        /// </summary>
        public int IterationCountForFirstStepOfGA { get; set; } = 15;

        /// <summary>
        /// Количество полных тактов ГА
        /// </summary>
        public int CountOfWholeStepsOfGA { get; set; } = 6;

        /// <summary>
        /// Количество точек на частотной оси
        /// </summary>
        public int PointsCountAtFrequencyAxle { get; set; } = 50;

        /// <summary>
        /// Положительное отклонение ЧХ
        /// </summary>
        public double PositiveDeviationOfTheFrequencyCharacteristic { get; set; } = 1;

        /// <summary>
        /// Отрицательное отклонение ЧХ
        /// </summary>
        public double NegativeDeviationOfTheFrequencyCharacteristic { get; set; } = 1;

        /// <summary>
        /// Минимальная частота
        /// </summary>
        public double MinFrequency { get; set; } = 2;

        /// <summary>
        /// Максимальная частота
        /// </summary>
        public double MaxFrequency { get; set; } = 5;

        /// <summary>
        /// Нижний предел диапазона изменения частот
        /// </summary>
        public double MinLevelOfFrequencyCharacteristic { get; set; } = -35;

        /// <summary>
        /// Верхний предел диапазона изменения частот
        /// </summary>
        public double MaxLevelOfFrequencyCharacteristic { get; set; } = -35;

        /// <summary>
        /// Технологический параметр G
        /// </summary>
        public double G { get; set; } = 0.01;

        /// <summary>
        /// Технологический параметр Rp  
        /// </summary>
        public string Rp { get; set; } = "10^9";

        /// <summary>
        /// Технологический параметр Rk
        /// </summary>
        public double Rk { get; set; } = 5;

        /// <summary>
        /// Экземпляр регулярного выражения соответствующее целому числу в целой степени
        /// </summary>
        private static readonly Regex _exponentNumberRegex = new Regex("^[-+]?[0-9]*[/^]?[0-9]+$");

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;

                switch (columnName)
                {
                    case nameof(IterationCountForFirstStepOfGA):
                        if (IterationCountForFirstStepOfGA < 0)
                        {
                            error = "Некорректный ввод";
                        }
                        break;
                    case nameof(CountOfWholeStepsOfGA):
                        if (CountOfWholeStepsOfGA < 0)
                        {
                            error = "Некорректный ввод";
                        }
                        break;
                    case nameof(PointsCountAtFrequencyAxle):
                        if (PointsCountAtFrequencyAxle < 0)
                        {
                            error = "Некорректный ввод";
                        }
                        break;
                    case nameof(PositiveDeviationOfTheFrequencyCharacteristic):
                        if (PositiveDeviationOfTheFrequencyCharacteristic < 0)
                        {
                            error = "Некорректный ввод";
                        }
                        break;
                    case nameof(NegativeDeviationOfTheFrequencyCharacteristic):
                        if (NegativeDeviationOfTheFrequencyCharacteristic < 0)
                        {
                            error = "Некорректный ввод";
                        }
                        break;
                    case nameof(MinFrequency):

                        break;
                    case nameof(MaxFrequency):

                        break;
                    case nameof(MinLevelOfFrequencyCharacteristic):

                        break;
                    case nameof(MaxLevelOfFrequencyCharacteristic):

                        break;
                    case nameof(G):

                        break;
                    case nameof(Rp):
                        if (!_exponentNumberRegex.IsMatch(Rp))
                        {
                            error = "Некорректный ввод";
                        }                   
                        break;
                    case nameof(Rk):

                        break;
                }

                return error;
            }
        }

        public string Error
        {
            get { return "Некорректный ввод"; }
        }
    }
}
