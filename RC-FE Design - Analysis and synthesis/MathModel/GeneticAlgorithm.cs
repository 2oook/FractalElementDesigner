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
        public GeneticAlgorithm(StructureSchemeSynthesisParameters synthesisParameters, int populationCount)
        {
            PopulationCount = populationCount;
            PopulationCountToMutate = (int)(populationCount * MutateCoefficient);

            IterationCountForFirstStepOfGA = synthesisParameters.IterationCountForFirstStepOfGA;
            CountOfWholeStepsOfGA = synthesisParameters.CountOfWholeStepsOfGA;
            PointsCountAtFrequencyAxle = synthesisParameters.PointsCountAtFrequencyAxle;
            PositiveDeviationOfTheFrequencyCharacteristic = synthesisParameters.PositiveDeviationOfTheFrequencyCharacteristic;
            NegativeDeviationOfTheFrequencyCharacteristic = synthesisParameters.NegativeDeviationOfTheFrequencyCharacteristic;
            MinFrequencyLn = synthesisParameters.MinFrequencyLn;
            MaxFrequencyLn = synthesisParameters.MaxFrequencyLn;
            MinLevelOfFrequencyCharacteristic = synthesisParameters.MinLevelOfFrequencyCharacteristic;
            MaxLevelOfFrequencyCharacteristic = synthesisParameters.MaxLevelOfFrequencyCharacteristic;

            // инициализировать диапазон кодов для произведения мутаций
            foreach (var connectionNumber in FElementScheme.AllowablePinsConnections.Keys.OrderBy(x => x))
            {
                var gnd = FElementScheme.AllowablePinsConnections[connectionNumber];
                ConnectionCodes.Add(connectionNumber, gnd.PEVector.Keys.OrderBy(x => x).ToList());
            }

            MaxConnectionCodeValue = ConnectionCodes.Keys.Max();
            MinConnectionCodeValue = ConnectionCodes.Keys.Min();
        }

        private Dictionary<int, List<int>> ConnectionCodes = new Dictionary<int, List<int>>();

        private int MaxConnectionCodeValue;

        private int MinConnectionCodeValue;

        private int PopulationCount;

        private int PopulationCountToMutate;

        public double MutateCoefficient { get; set; } = 0.3;

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
        public List<FElementScheme> Population;

        private FElementScheme SchemePrototype;

        /// <summary>
        /// Метод для инициализации популяции
        /// </summary>
        public void InitiatePopulation(FElementScheme schemePrototype) 
        {
            SchemePrototype = schemePrototype;

            var schemes = new List<FElementScheme>();

            // инициализировать популяцию
            for (int i = 0; i < PopulationCount; i++)
            {
                var newScheme = schemePrototype.DeepClone() as FElementScheme;

                schemes.Add(newScheme);
                this.InitiateIndividual(newScheme.Model);
            }

            Population = schemes;
        }

        public void Fit(FESchemeModel model)
        {

        }

        public void Select()
        {

        }

        public void InitiateIndividual(FESchemeModel model)
        {
            foreach (var connection in model.InnerConnections)
            {
                // сгенерировать код соединения
                var connectionCode = random.Next(MinConnectionCodeValue, MaxConnectionCodeValue);
                // найти список возможных кодов заземлений этого соединения
                var groundCodes = ConnectionCodes[connectionCode];
                // сгенерировать код заземления
                var groundCode = random.Next(groundCodes[0], groundCodes[groundCodes.Count - 1]);

                connection.ConnectionType = connectionCode;
                connection.PEType = groundCode;
            }
        }

        public void MutatePopulation()
        {
            // для отладки
            var schemesToMutate = new List<int>();

            var mutateConnectionIndices = new List<int>();

            for (int i = 0; i < PopulationCountToMutate; i++)
            {
                var schemeIndexToMutate = random.Next(i, PopulationCount - 1);
                schemesToMutate.Add(schemeIndexToMutate);
                var mutateConnectionsCount = random.Next(1, SchemePrototype.Model.InnerConnections.Count);

                // выбор соединений для мутации
                for (int k = 0; k <= mutateConnectionsCount; k++)
                {
                    var mutateIndex = random.Next(0, Population[schemeIndexToMutate].Model.InnerConnections.Count - 1);
                    mutateConnectionIndices.Add(mutateIndex);
                }              

                for (int j = 0; j < mutateConnectionIndices.Count; j++)
                {
                    // сгенерировать код соединения
                    var connectionCode = random.Next(MinConnectionCodeValue, MaxConnectionCodeValue);
                    // найти список возможных кодов заземлений этого соединения
                    var groundCodes = ConnectionCodes[connectionCode];
                    // сгенерировать код заземления
                    var groundCode = random.Next(groundCodes[0], groundCodes[groundCodes.Count - 1]);

                    Population[schemeIndexToMutate].Model.InnerConnections[mutateConnectionIndices[j]].ConnectionType = connectionCode;
                    Population[schemeIndexToMutate].Model.InnerConnections[mutateConnectionIndices[j]].PEType = groundCode;                     
                }

                mutateConnectionIndices.Clear();
            }
        }

        public void MutateIndividual()
        {

        }

        public FElementScheme Cross(FElementScheme first, FElementScheme second)
        {
            var newScheme = first.DeepClone() as FElementScheme;

            var mutateConnectionsCount = random.Next(0, (SchemePrototype.Model.InnerConnections.Count / 2) + 1);

            for (int i = 0; i < mutateConnectionsCount; i++)
            {
                var mutateConnectionIndex = random.Next(0, SchemePrototype.Model.InnerConnections.Count -1);

                newScheme.Model.InnerConnections[mutateConnectionIndex].ConnectionType = second.Model.InnerConnections[mutateConnectionIndex].ConnectionType;
                newScheme.Model.InnerConnections[mutateConnectionIndex].PEType = second.Model.InnerConnections[mutateConnectionIndex].PEType;
            }

            return newScheme;
        }
    }
}
