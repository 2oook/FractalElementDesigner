﻿using RC_FE_Design___Analysis_and_synthesis.MathModel;
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
        // TODO ---- этот конструктор нужно заменить вводом
        public StructureSchemeSynthesisParameters()
        {
            // TODO
            // осуществить ввод из формы

            var G = this.G;
            var Rk = this.Rk;

            var val = this.Rp.Split('^');
            var exp_base = double.Parse(val[0]);
            var exp = double.Parse(val[1]);

            var Rp = Math.Pow(exp_base, exp);

            var sec1 = new FESection(new FESectionParameters()
            {
                C = 0.000_000_000_2,
                R = 118_718,
                N = 13.1,
                L = 3,
                G = 3,
                Rk = Rk,
                Rp = Rp             
            }, 
            new List<Pin>()
            {
                new Pin() { Number = 0, Orientation = PinOrientationEnum.Left },
                new Pin() { Number = 4, Orientation = PinOrientationEnum.Right },
                new Pin() { Number = 5, Orientation = PinOrientationEnum.Right },
                new Pin() { Number = 3, Orientation = PinOrientationEnum.Left }
            }, 
            1);

            FESections.Add(sec1);

            var sec2 = new FESection(new FESectionParameters()
            {
                C = 0.000_000_000_2,
                R = 118_718,
                N = 13.1,
                L = 9.5,
                G = 9.5,
                Rk = Rk,
                Rp = Rp
            },
            new List<Pin>()
            {
                new Pin() { Number = 6, Orientation = PinOrientationEnum.Left },
                new Pin() { Number = 8, Orientation = PinOrientationEnum.Right },
                new Pin() { Number = 9, Orientation = PinOrientationEnum.Right },
                new Pin() { Number = 7, Orientation = PinOrientationEnum.Left }
            },
            2);

            FESections.Add(sec2);

            var sec3 = new FESection(new FESectionParameters()
            {
                C = 0.000_000_000_2,
                R = 118_718,
                N = 13.1,
                L = 8.1,
                G = 8.1,
                Rk = Rk,
                Rp = Rp
            }, 
            new List<Pin>()
            {
                new Pin() { Number = 10, Orientation = PinOrientationEnum.Left },
                new Pin() { Number = 12, Orientation = PinOrientationEnum.Right },
                new Pin() { Number = 13, Orientation = PinOrientationEnum.Right },
                new Pin() { Number = 11, Orientation = PinOrientationEnum.Left }
            }, 
            3);

            FESections.Add(sec3);

            var sec4 = new FESection(new FESectionParameters()
            {
                C = 0.000_000_000_2,
                R = 118_718,
                N = 13.1,
                L = 7.8,
                G = 7.8,
                Rk = Rk,
                Rp = Rp
            },
            new List<Pin>()
            {
                new Pin() { Number = 14, Orientation = PinOrientationEnum.Left },
                new Pin() { Number = 1, Orientation = PinOrientationEnum.Right },
                new Pin() { Number = 2, Orientation = PinOrientationEnum.Right },
                new Pin() { Number = 15, Orientation = PinOrientationEnum.Left }
            },
            4);

            FESections.Add(sec4);
            // TODO
        }

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
        /// Секции ФРЭ
        /// </summary>
        public List<FESection> FESections { get; set; } = new List<FESection>();

        /// <summary>
        /// Экземпляр регулярного выражения соответствующее целому числу в целой степени
        /// </summary>
        private static readonly Regex _exponentNumberRegex = new Regex("^[-+]?[0-9]*[/^]?[0-9]+$");

        private Dictionary<string, string> errorCollection = new Dictionary<string, string>();

        // Индексатор ошибки валидации
        public string this[string columnName]
        {
            get
            {
                string error = null;

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

                if (error != null && !errorCollection.ContainsKey(columnName))
                    errorCollection.Add(columnName, error);
                if (error == null && errorCollection.ContainsKey(columnName))
                    errorCollection.Remove(columnName);

                return error;
            }
        }

        /// <summary>
        /// Общая ошибка валидации ввода
        /// </summary>
        public string Error
        {
            get 
            {
                if (errorCollection.Count == 0)
                    return null;

                StringBuilder errorList = new StringBuilder();
                var errorMessages = errorCollection.Values.GetEnumerator();
                while (errorMessages.MoveNext())
                    errorList.AppendLine(errorMessages.Current);

                return errorList.ToString();
            }
        }
    }
}
