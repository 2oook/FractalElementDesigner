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
    class GeneticAlgorithm
    {
        public GeneticAlgorithm(StructureSchemeSynthesisParameters synthesisParameters)
        {
            IterationCountForFirstStepOfGA = synthesisParameters.IterationCountForFirstStepOfGA;
            CountOfWholeStepsOfGA = synthesisParameters.CountOfWholeStepsOfGA;
            PointsCountAtFrequencyAxle = synthesisParameters.PointsCountAtFrequencyAxle;
            PositiveDeviationOfTheFrequencyCharacteristic = synthesisParameters.PositiveDeviationOfTheFrequencyCharacteristic;
            NegativeDeviationOfTheFrequencyCharacteristic = synthesisParameters.NegativeDeviationOfTheFrequencyCharacteristic;
            MinFrequencyLn = synthesisParameters.MinFrequencyLn;
            MaxFrequencyLn = synthesisParameters.MaxFrequencyLn;
            MinLevelOfFrequencyCharacteristic = synthesisParameters.MinLevelOfFrequencyCharacteristic;
            MaxLevelOfFrequencyCharacteristic = synthesisParameters.MaxLevelOfFrequencyCharacteristic;
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
        public double MinFrequencyLn { get; set; }

        /// <summary>
        /// Максимальная частота
        /// </summary>
        public double MaxFrequencyLn { get; set; }

        /// <summary>
        /// Нижний предел диапазона изменения частот
        /// </summary>
        public double MinLevelOfFrequencyCharacteristic { get; set; }

        /// <summary>
        /// Верхний предел диапазона изменения частот
        /// </summary>
        public double MaxLevelOfFrequencyCharacteristic { get; set; }

        /// <summary>
        /// Объект для генерации случайных чисел
        /// </summary>
        private Random random = new Random(1);

        /// <summary>
        /// Популяция
        /// </summary>
        public List<FElementScheme> Population { get; set; }

        /// <summary>
        /// Метод для инициализации популяции
        /// </summary>
        public List<FElementScheme> InitiatePopulation(FElementScheme schemePrototype, int populationCount) 
        {
            var schemes = new List<FElementScheme>();

            // инициализировать популяцию
            for (int i = 0; i < populationCount; i++)
            {
                var newScheme = schemePrototype.DeepClone() as FElementScheme;

                schemes.Add(newScheme);
                this.Mutate(newScheme.Model);
            }

            return schemes;
        }

        public void Fit(FESchemeModel model)
        {

        }

        public void Select()
        {

        }

        public void Mutate(FESchemeModel model)
        {
            //model.
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
