using RC_FE_Design___Analysis_and_synthesis.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Класс представляет генетический алгоритм
    /// </summary>
    public class GeneticAlgorithm
    {
        public GeneticAlgorithm(StructureSchemeSynthesisParameters synthesisParameters)
        {
            IterationCountForFirstStepOfGA = synthesisParameters.IterationCountForFirstStepOfGA;
            CountOfWholeStepsOfGA = synthesisParameters.CountOfWholeStepsOfGA;
            PointsCountAtFrequencyAxle = synthesisParameters.PointsCountAtFrequencyAxle;
            PositiveDeviationOfTheFrequencyCharacteristic = synthesisParameters.PositiveDeviationOfTheFrequencyCharacteristic;
            NegativeDeviationOfTheFrequencyCharacteristic = synthesisParameters.NegativeDeviationOfTheFrequencyCharacteristic;
            MinFrequency = synthesisParameters.MinFrequency;
            MaxFrequency = synthesisParameters.MaxFrequency;
            MinLevelOfFrequencyCharacteristic = synthesisParameters.MinLevelOfFrequencyCharacteristic;
            MaxLevelOfFrequencyCharacteristic = synthesisParameters.MaxLevelOfFrequencyCharacteristic;
            G = synthesisParameters.G;
            Rk = synthesisParameters.Rk;

            var val = synthesisParameters.Rp.Split('^');
            var exp_base = double.Parse(val[0]);
            var exp = double.Parse(val[1]);

            Rp = Math.Pow(exp_base, exp);
        }

        /// <summary>
        /// Количество итераций 1-го такта ГА
        /// </summary>
        public int IterationCountForFirstStepOfGA { get; set; }

        /// <summary>
        /// Количество полных тактов ГА
        /// </summary>
        public int CountOfWholeStepsOfGA { get; set; }

        /// <summary>
        /// Количество точек на частотной оси
        /// </summary>
        public int PointsCountAtFrequencyAxle { get; set; }

        /// <summary>
        /// Положительное отклонение ЧХ
        /// </summary>
        public double PositiveDeviationOfTheFrequencyCharacteristic { get; set; }

        /// <summary>
        /// Отрицательное отклонение ЧХ
        /// </summary>
        public double NegativeDeviationOfTheFrequencyCharacteristic { get; set; }

        /// <summary>
        /// Минимальная частота
        /// </summary>
        public double MinFrequency { get; set; }

        /// <summary>
        /// Максимальная частота
        /// </summary>
        public double MaxFrequency { get; set; }

        /// <summary>
        /// Нижний предел диапазона изменения частот
        /// </summary>
        public double MinLevelOfFrequencyCharacteristic { get; set; }

        /// <summary>
        /// Верхний предел диапазона изменения частот
        /// </summary>
        public double MaxLevelOfFrequencyCharacteristic { get; set; }

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

        private Random random = new Random();

        /// <summary>
        /// Популяция
        /// </summary>
        public List<FElementScheme> Population { get; set; }

        /// <summary>
        /// Метод для инициализации популяции
        /// </summary>
        public void InitiatePopulation() 
        {

        }

        public void Fit(FElementScheme scheme)
        {

        }

        public void Select()
        {

        }

        public void Mutate_C()
        {

        }

        public void Mutate_P()
        {

        }

        public void Cross_C()
        {

        }

        public void Cross_P()
        {

        }
    }
}
