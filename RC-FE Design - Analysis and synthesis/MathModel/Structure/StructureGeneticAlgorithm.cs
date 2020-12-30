using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.RCWorkbenchLibrary;
using FractalElementDesigner.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Scheme = structure.Scheme;

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


        // Число потоков
        private int ThreadCount = Environment.ProcessorCount;

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
        /// Схема
        /// </summary>
        private FElementScheme Scheme;

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
                MutateIndividual(newStructure);
            }

            Population = structures;
        }

        public void MutateIndividual(RCStructure structure) 
        {
            // конструкция подлежащая мутации
            var structureToMutate = structure;

            var mutateSegmentsCount = random.Next(1, (int)(SegmentsCountToMutate * 0.15)); // максимальное число мутируемых сегментов составляет 60 процентов 

            // выбор сегментов для мутации
            for (int k = 0; k <= mutateSegmentsCount; k++)
            {
                for (int z = 0; z < mutateSegmentsCount; z++)
                {
                    var _r = random.Next(1, structureToMutate.Segments.Count() - 1);
                    var _c = random.Next(1, structureToMutate.Segments.First().Count - 1);

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

            StructureCreator.GetCellTypesFromStructureTypes(structureToMutate);
        }

        // Метод для мутации популяции
        public void MutatePopulation() 
        {
            for (int i = 0; i < PopulationCountToMutate; i++) 
            {
                var structureIndexToMutate = random.Next(i, PopulationCount - 1);

                var structureToMutate = Population[structureIndexToMutate];

                MutateIndividual(structureToMutate);
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

            // создать структуру на стороне библиотеки
            ByRCWorkbenchStructureCreator.CreateStructureStraightByScheme(Scheme, structure);

            structure.Scheme = Scheme;

            // пронумеровать контактные площадки 
            StructureCreator.NumerateContactPlatesByScheme(structure);

            // проверка
            int outer_pins_count = RCWorkbenchLibraryEntry.GetCPQuantity();

            RCWorkbenchLibraryEntry.GetNodesNumeration(nodesNumerationFlat);

            // получить количество узлов
            var nodesCount = RCWorkbenchLibraryEntry.GetNodesQuantity();

            RCWorkbenchLibraryEntry.DeleteStructureStraight();

            // восстановить плоский массив нумерации узлов
            var nodesNumeration = RCWorkbenchIntercommunicationHelper.UnflatNumerationArray(layerCount, horizontalRange, verticalRange, nodesNumerationFlat);

            var structurePhaseResponseCalculator = new StructurePhaseResponseCalculator(structure, horizontalStructureDimensionValue, verticalStructureDimensionValue, nodesCount, nodesNumeration);


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

        public void ParallelFit(RCStructure structure)
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

            // создать структуру на стороне библиотеки
            ByRCWorkbenchStructureCreator.CreateStructureStraightByScheme(Scheme, structure);

            structure.Scheme = Scheme;

            // пронумеровать контактные площадки 
            StructureCreator.NumerateContactPlatesByScheme(structure);

            // проверка
            int outer_pins_count = RCWorkbenchLibraryEntry.GetCPQuantity();

            RCWorkbenchLibraryEntry.GetNodesNumeration(nodesNumerationFlat);

            // получить количество узлов
            var nodesCount = RCWorkbenchLibraryEntry.GetNodesQuantity();

            RCWorkbenchLibraryEntry.DeleteStructureStraight();

            // восстановить плоский массив нумерации узлов
            var nodesNumeration = RCWorkbenchIntercommunicationHelper.UnflatNumerationArray(layerCount, horizontalRange, verticalRange, nodesNumerationFlat);

            var structurePhaseResponseCalculator = new StructurePhaseResponseCalculator(structure, horizontalStructureDimensionValue, verticalStructureDimensionValue, nodesCount, nodesNumeration);

            var points = new List<(double frequency, double phase)>();

            int rate = 0;

            Func<object, int> action = (object obj) =>
            {
                // Объект с параметрами для потоков
                var to = (ThreadParameters)obj;

                var calculator = new StructurePhaseResponseCalculator(structure, horizontalStructureDimensionValue, verticalStructureDimensionValue, nodesCount, nodesNumeration);


                // Установить начальный индекс
                int i = to.startIndex;

                // Верхний предел цикла
                int count = i + to.indexesPool;

                double frequency = MinFrequency + (FrequencyIncrement * (i + 1));

                for (int j = (i + 1); j <= count; j++) 
                {
                    var phase = calculator.CalculatePhase(frequency);

                    // если точка попадает в окно
                    if (phase > LowerCharacteristicBound & phase < UpperCharacteristicBound)
                    {
                        to.rate++;
                    }

                    to.points.Add((frequency, phase));

                    frequency += FrequencyIncrement;
                }

                return 0;
            };

            // Если точек меньше чем количества потоков
            if (PointsCountAtFrequencyAxle < ThreadCount)
            {
                ThreadCount = 1;
            }

            // Пул индексов одного потока
            int indexesPool = (int)Math.Floor((double)PointsCountAtFrequencyAxle / ThreadCount);
            // Остаток индексов
            int lastIndexes = PointsCountAtFrequencyAxle - (indexesPool * ThreadCount);

            // Пока оставшиеся индексы больше чем пул индексов потока
            while (lastIndexes > indexesPool)
            {
                lastIndexes -= indexesPool;
                ThreadCount++;
            }

            // Список потоков
            var tasks = new List<Task<int>>();
            var tos = new List<ThreadParameters>();

            // Создать потоки
            for (int t = 0; t < ThreadCount; t++)
            {
                var to = new ThreadParameters();

                // Если это последний блок итераций
                if (t == ThreadCount - 1)
                {
                    to.indexesPool = indexesPool + lastIndexes;
                }
                else
                {
                    to.indexesPool = indexesPool;
                }

                to.startIndex = t * indexesPool;

                // Добавить задачу в список задач и запустить её на выполнение в отдельном потоке
                tasks.Add(Task<int>.Factory.StartNew(action, to));
                tos.Add(to);
            }

            try
            {
                // Подождать все задачи
                Task.WaitAll(tasks.ToArray());

                foreach (var to in tos)
                {
                    points = points.Union(to.points).ToList();
                    rate += to.rate;
                }

                points = points.OrderBy(x => x.frequency).ToList();

                Debug.WriteLine("WaitAll() has not thrown exceptions.");
            }
            catch (AggregateException e)
            {
                Debug.WriteLine("The following exceptions have been thrown by WaitAll()");

                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    Debug.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                }

                throw e;
            }

            structure.StateInGA.Rate = rate;
            structure.PhaseResponsePoints = points;
        }

        // Метод для оценки популяции
        public void RatePopulation()
        {
            foreach (var structure in Population)
            {
                ParallelFit(structure);
            }
        }

        // Метод для отбора популяции
        public void SelectPopulation()
        {
            var newPopulation = Population.OrderByDescending(x => x.StateInGA.Rate).Take(PopulationCount).ToList();

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

    public class ThreadParameters
    {
        /// <summary>
        /// Номер блока итераций
        /// </summary>
        public int startIndex;

        /// <summary>
        /// Блок индексов для итераций
        /// </summary>
        public int indexesPool;


        public int rate = 0;


        public List<(double frequency, double phase)> points = new List<(double frequency, double phase)>();
    }
}
