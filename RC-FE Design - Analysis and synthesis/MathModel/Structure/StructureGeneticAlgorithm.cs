using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.RCWorkbenchLibrary;
using FractalElementDesigner.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel.Structure
{
    class StructureGeneticAlgorithm
    {
        /// <summary>
        /// Событие выполнения части работы
        /// </summary>
        public static event Action<double> OnDoWork;

        public StructureGeneticAlgorithm(int populationCount, StructureSchemeSynthesisParameters synthesisParameters, RCStructure structure)
        {
            StructurePrototype = structure;

            PopulationCount = populationCount;
            PopulationCountToMutate = (int)(populationCount * MutateCoefficient);

            // найти максимальное число сегментов для мутации
            foreach (var row in structure.Segments)
            {
                foreach (var segment in row)
                {
                    switch (segment.SegmentType)
                    {
                        case StructureSegmentTypeEnum.EMPTY:
                            SegmentsCountToMutate++;
                            break;
                        case StructureSegmentTypeEnum.R_C_NR:
                            SegmentsCountToMutate++;
                            break;
                        case StructureSegmentTypeEnum.Rv:
                            SegmentsCountToMutate++;
                            break;
                        case StructureSegmentTypeEnum.Rn:
                            SegmentsCountToMutate++;
                            break;
                        default:
                            break;
                    }
                }
            }

            CountOfWholeStepsOfGA = synthesisParameters.CountOfWholeStepsOfGA;
            PointsCountAtFrequencyAxle = synthesisParameters.PointsCountAtFrequencyAxle;
            PositiveDeviationOfTheFrequencyCharacteristic = synthesisParameters.PositiveDeviationOfTheFrequencyCharacteristic;
            NegativeDeviationOfTheFrequencyCharacteristic = synthesisParameters.NegativeDeviationOfTheFrequencyCharacteristic;

            MinFrequency = Math.Pow(10, synthesisParameters.MinFrequencyLn);
            MaxFrequency = Math.Pow(10, synthesisParameters.MaxFrequencyLn);

            FrequencyIncrement = (MaxFrequency - MinFrequency) / PointsCountAtFrequencyAxle;

            MinLevelOfFrequencyCharacteristic = synthesisParameters.MinLevelOfFrequencyCharacteristic;
            MaxLevelOfFrequencyCharacteristic = synthesisParameters.MaxLevelOfFrequencyCharacteristic;

            // сформировать окно корректности ФЧХ
            if (MaxLevelOfFrequencyCharacteristic < MinLevelOfFrequencyCharacteristic)
            {
                // из меньшего вычитаем, к большему прибавляем
                LowerCharacteristicBound = MaxLevelOfFrequencyCharacteristic - NegativeDeviationOfTheFrequencyCharacteristic;
                UpperCharacteristicBound = MinLevelOfFrequencyCharacteristic + PositiveDeviationOfTheFrequencyCharacteristic;
            }
            else
            {
                // из меньшего вычитаем, к большему прибавляем
                LowerCharacteristicBound = MinLevelOfFrequencyCharacteristic - NegativeDeviationOfTheFrequencyCharacteristic;
                UpperCharacteristicBound = MaxLevelOfFrequencyCharacteristic + PositiveDeviationOfTheFrequencyCharacteristic;
            }
        }

        /// <summary>
        /// Число сегментов для мутации
        /// </summary>
        private int SegmentsCountToMutate;

        /// <summary>
        /// Число популяции
        /// </summary>
        private int PopulationCount;

        /// <summary>
        /// Число особей для мутации
        /// </summary>
        private int PopulationCountToMutate;

        /// <summary>
        /// Число для инкремента частоты
        /// </summary>
        private double FrequencyIncrement;

        /// <summary>
        /// Нижняя граница окна
        /// </summary>
        private double LowerCharacteristicBound;

        /// <summary>
        /// Верхняя граница окна
        /// </summary>
        private double UpperCharacteristicBound;

        /// <summary>
        /// Коэффициент мутации популяции
        /// </summary>
        public double MutateCoefficient { get; set; } = 0.3;

        /// <summary>
        /// Количество полных тактов ГА
        /// </summary>
        public int CountOfWholeStepsOfGA;

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
        /// Объект для генерации случайных чисел
        /// </summary>
        private Random random = new Random();

        /// <summary>
        /// Популяция
        /// </summary>
        public List<RCStructure> Population;

        /// <summary>
        /// Прототип конструкции
        /// </summary>
        private RCStructure StructurePrototype;

        /// <summary>
        /// Метод для инициализации популяции
        /// </summary>
        public void InitiatePopulation(RCStructure structurePrototype) 
        {
            StructurePrototype = structurePrototype;

            var structures = new List<RCStructure>();

            // инициализировать популяцию
            for (int i = 0; i < PopulationCount; i++)
            {
                var newStructure = structurePrototype.DeepClone() as RCStructure;

                structures.Add(newStructure);
                InitiateIndividual(newStructure);
            }

            Population = structures;
        }

        public void InitiateIndividual(RCStructure structure) 
        {

        }

        // Метод для мутации популяции
        public void MutatePopulation() 
        {
            for (int i = 0; i < PopulationCountToMutate; i++) 
            {
                var structureIndexToMutate = random.Next(i, PopulationCount - 1);

                var mutateSegmentsCount = random.Next(1, (int)(SegmentsCountToMutate * 0.6)); // максимальное число мутируемых сегментов составляет 60 процентов 

                // выбор сегментов для мутации
                for (int k = 0; k <= mutateSegmentsCount; k++)
                {
                    // конструкция подлежащая мутации
                    var structureToMutate = Population[structureIndexToMutate];

                    for (int z = 0; z < mutateSegmentsCount; z++)
                    {
                        var _r = random.Next(1, structureToMutate.Segments.Count()-1);
                        var _c = random.Next(1, structureToMutate.Segments.First().Count-1);

                        if (structureToMutate.Segments[_r][_c].SegmentType == StructureSegmentTypeEnum.EMPTY || 
                            structureToMutate.Segments[_r][_c].SegmentType == StructureSegmentTypeEnum.R_C_NR ||
                            structureToMutate.Segments[_r][_c].SegmentType == StructureSegmentTypeEnum.Rv ||
                            structureToMutate.Segments[_r][_c].SegmentType == StructureSegmentTypeEnum.Rn)
                        {
                            var structureTypeNumber = random.Next(0, 3);

                            switch (structureTypeNumber)
                            {
                                case 0:
                                    structureToMutate.Segments[_r][_c].SegmentType = StructureSegmentTypeEnum.EMPTY;
                                    break;
                                case 1:
                                    structureToMutate.Segments[_r][_c].SegmentType = StructureSegmentTypeEnum.R_C_NR;
                                    break;
                                case 2:
                                    structureToMutate.Segments[_r][_c].SegmentType = StructureSegmentTypeEnum.Rv;
                                    break;
                                case 3:
                                    structureToMutate.Segments[_r][_c].SegmentType = StructureSegmentTypeEnum.Rn;
                                    break;
                            }
                        }                     
                    }
                }
            }
        }

        // Метод для скрещивания двух конструкций
        public RCStructure Cross(RCStructure first, RCStructure second) 
        {
            // Взять за основу первую схему
            var newStructure = first.DeepClone() as RCStructure;

            var crossSegmentsCount = random.Next(1, (int)(SegmentsCountToMutate * 0.6));

            for (int z = 0; z < crossSegmentsCount; z++)
            {
                var _r = random.Next(1, newStructure.Segments.Count() - 1);
                var _c = random.Next(1, newStructure.Segments.First().Count - 1);

                newStructure.Segments[_r][_c].SegmentType = second.Segments[_r][_c].SegmentType;
            }

            return newStructure;
        }


        // Метод для скрещивания популяции
        public void CrossPopulation()
        {
            var randomizedPopulation = Population.OrderBy(x => random.Next()).ToList();

            var randomizedPopulationCount = randomizedPopulation.Count - (randomizedPopulation.Count % 2);

            for (int i = 0; i < randomizedPopulationCount; i += 2)
            {
                var structure = Cross(randomizedPopulation[i], randomizedPopulation[i + 1]);
                Population.Add(structure);
            }
        }

        public void Fit(RCStructure structure)
        {
            // число ячеек по горизонтали структуры
            var horizontalStructureDimensionValue = structure.Segments.First().Count - 2;
            // число ячеек по вертикали структуры
            var verticalStructureDimensionValue = structure.Segments.Count - 2;

            var layerCount = structure.StructureLayers.Count;
            var horizontalRange = (horizontalStructureDimensionValue + 1);
            var verticalRange = (verticalStructureDimensionValue + 1);
            var arrayDimension = layerCount * horizontalRange * verticalRange;
            var nodesNumerationFlat = new int[arrayDimension];

            // проверка
            int outer_pins_count = RCWorkbenchLibraryEntry.GetCPQuantity();

            RCWorkbenchLibraryEntry.GetNodesNumeration(nodesNumerationFlat);

            // получить количество узлов
            var nodesCount = RCWorkbenchLibraryEntry.GetNodesQuantity();

            RCWorkbenchLibraryEntry.DeleteStructureStraight();

            // восстановить плоский массив нумерации узлов
            var nodesNumeration = RCWorkbenchIntercommunicationHelper.UnflatNumerationArray(layerCount, horizontalRange, verticalRange, nodesNumerationFlat);

            var structurePhaseResponseCalculator = new StructurePhaseResponseCalculator(structure, horizontalStructureDimensionValue, verticalStructureDimensionValue, nodesCount, nodesNumeration);



            // TODO // Распаралеллить

            var points = new List<(double frequency, double phase)>();

            int rate = 0;

            double frequency = MinFrequency;
            // цикл по частотам
            for (int i = 0; i < PointsCountAtFrequencyAxle; i++)
            {
                var phase = structurePhaseResponseCalculator.CalculatePhase(frequency);

                // если точка попадает в окно
                if (phase > LowerCharacteristicBound & phase < UpperCharacteristicBound)
                {
                    rate++;
                }

                points.Add((frequency, phase));

                frequency += FrequencyIncrement;
            }

            structure.StateInGA.Rate = rate;
            structure.PhaseResponsePoints = points;
        }

        // Метод для оценки популяции
        public void RatePopulation()
        {
            foreach (var structure in Population)
            {
                Fit(structure);
            }
        }

        // Метод для отбора популяции
        public void SelectPopulation()
        {
            var newPopulation = Population.OrderByDescending(x => x.StateInGA.Rate).Take(100).ToList();

            Population = newPopulation;
        }

        // Метод для получения популяции
        public List<RCStructure> GetPopulation()
        {
            return Population;
        }

        public void Start() 
        {
            InitiatePopulation(StructurePrototype);

            double increment = 100f / CountOfWholeStepsOfGA;

            for (int i = 0; i < CountOfWholeStepsOfGA; i++)
            {
                OnDoWork(increment * (i + 1));

                MutatePopulation();

                CrossPopulation();

                RatePopulation();

                SelectPopulation();
            }
        }
    }
}
