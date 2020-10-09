using FractalElementDesigner.StructureSchemeSynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Класс представляет генетический алгоритм с одной ступенью
    /// </summary>
    class SingleStageGeneticAlgorithm : IGeneticAlgorithm
    {
        /// <summary>
        /// Событие выполнения части работы
        /// </summary>
        public static event Action<double> OnDoWork;

        // Конструктор
        public SingleStageGeneticAlgorithm(StructureSchemeSynthesisParameters synthesisParameters, int populationCount, FElementScheme schemePrototype)
        {
            SchemePrototype = schemePrototype;

            PopulationCount = populationCount;
            PopulationCountToMutate = (int)(populationCount * MutateCoefficient);

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

        /// <summary>
        /// Максимальный код соединения (определяется один раз для повышения эффективности)
        /// </summary>
        private int MaxConnectionCodeValue;

        /// <summary>
        /// Минимальный код соединения (определяется один раз для повышения эффективности)
        /// </summary>
        private int MinConnectionCodeValue;

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

        // Метод для инициализации индивида
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

        // Метод для мутации популяции
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

                    MutateOuterPins(Population[schemeIndexToMutate].Model.OuterPins);     
                }

                mutateConnectionIndices.Clear();
            }
        }

        // Метод для мутации внешних выводов схемы
        public void MutateOuterPins(List<OuterPin> outerPins)
        {
            var outerPinsCopy = outerPins.Select((x, i) => new { i, x.State }).ToList();

            var main_rand = random.Next(0, 100);

            var ins = outerPins.Where(x => x.State == OuterPinState.In).ToList();
            var gnds = outerPins.Where(x => x.State == OuterPinState.Gnd).ToList();
            var conns = outerPins.Where(x => x.State == OuterPinState.Con).ToList();

            // мутируем in
            if (main_rand <= 30)
            {
                var rnd_for_choice = random.Next(0, 100);

                // обмен
                if (rnd_for_choice <= 40 & ins.Count == 1)
                {
                    int rand_in = random.Next(0, outerPins.Count - 1);
                    ins[0].State = outerPins[rand_in].State;
                    outerPins[rand_in].State = OuterPinState.In;
                }
                // уменьшение или увеличение количества
                else
                {
                    var in_count = random.Next(1, 3);

                    ins.ForEach(x => x.State = OuterPinState.Float);

                    while (in_count != 0)
                    {
                        int rand_in = random.Next(0, outerPins.Count - 1);

                        if (outerPins[rand_in].State == OuterPinState.Float)
                        {
                            outerPins[rand_in].State = OuterPinState.In;
                            in_count--;
                        }
                        else if (outerPins[rand_in].State == OuterPinState.Gnd & gnds.Count > 1)
                        {
                            outerPins[rand_in].State = OuterPinState.In;
                            in_count--;
                        }
                        else if (outerPins[rand_in].State == OuterPinState.Con)
                        {
                            conns.ForEach(x => x.State = OuterPinState.Float);
                            outerPins[rand_in].State = OuterPinState.In;
                            in_count--;
                        }
                    }
                }
            }
            // мутируем gnd
            else if (main_rand > 30 & main_rand <= 60)
            {
                var rnd_for_choice = random.Next(0, 100);

                // обмен
                if (rnd_for_choice <= 40 & gnds.Count == 1)
                {
                    int rand_in = random.Next(0, outerPins.Count - 1);
                    gnds[0].State = outerPins[rand_in].State;
                    outerPins[rand_in].State = OuterPinState.Gnd;
                }
                // уменьшение или увеличение количества
                else
                {
                    var in_count = random.Next(1, 3);

                    gnds.ForEach(x => x.State = OuterPinState.Float);

                    while (in_count != 0)
                    {
                        int rand_in = random.Next(0, outerPins.Count - 1);

                        if (outerPins[rand_in].State == OuterPinState.Float)
                        {
                            outerPins[rand_in].State = OuterPinState.Gnd;
                            in_count--;
                        }
                        else if (outerPins[rand_in].State == OuterPinState.In & ins.Count > 1)
                        {
                            outerPins[rand_in].State = OuterPinState.Gnd;
                            in_count--;
                        }
                        else if (outerPins[rand_in].State == OuterPinState.Con)
                        {
                            conns.ForEach(x => x.State = OuterPinState.Float);
                            outerPins[rand_in].State = OuterPinState.Gnd;
                            in_count--;
                        }
                    }
                }
            }
            // мутируем conn
            else
            {
                var rnd_for_choice = random.Next(0, 100);

                if (rnd_for_choice <= 40 & conns.Count > 1)
                {
                    int rand = random.Next(0, outerPins.Count - 1);
                    conns[0].State = outerPins[rand].State;
                    outerPins[rand].State = OuterPinState.Con;
                }
                else
                {
                    conns.ForEach(x => x.State = OuterPinState.Float);
                }
            }
        }

        public void MutateSchemeConnection(FElementScheme scheme, int connectionIndex)
        {
            // сгенерировать код соединения
            var connectionCode = random.Next(MinConnectionCodeValue, MaxConnectionCodeValue);
            // найти список возможных кодов заземлений этого соединения
            var groundCodes = ConnectionCodes[connectionCode];
            // сгенерировать код заземления
            var groundCode = random.Next(groundCodes[0], groundCodes[groundCodes.Count - 1]);

            scheme.Model.InnerConnections[connectionIndex].ConnectionType = connectionCode;
            scheme.Model.InnerConnections[connectionIndex].PEType = groundCode;
        }

        // Метод для скрещивания двух схем
        public FElementScheme Cross(FElementScheme first, FElementScheme second)
        {
            // Взять за основу первую схему
            var newScheme = first.DeepClone() as FElementScheme;

            // число соединений для скрещивания
            var crossConnectionsCount = random.Next(0, (SchemePrototype.Model.InnerConnections.Count / 2) + 1);

            for (int i = 0; i < crossConnectionsCount; i++)
            {
                var mutateConnectionIndex = random.Next(0, SchemePrototype.Model.InnerConnections.Count -1);

                // добавить гены второй схемы в первую
                newScheme.Model.InnerConnections[mutateConnectionIndex].ConnectionType = second.Model.InnerConnections[mutateConnectionIndex].ConnectionType;
                newScheme.Model.InnerConnections[mutateConnectionIndex].PEType = second.Model.InnerConnections[mutateConnectionIndex].PEType;
            }

            // скрестить внешнее подключение схем
            var main_rand = random.Next(0, 100);

            if (main_rand <= 50) 
            {
                newScheme.Model.OuterPins = first.Model.OuterPins;
            }
            else
            {
                newScheme.Model.OuterPins = second.Model.OuterPins;
            }

            return newScheme;
        }

        // Метод для скрещивания популяции
        public void CrossPopulation() 
        {
            var randomizedPopulation = Population.OrderBy(x => random.Next()).ToList();

            var randomizedPopulationCount = randomizedPopulation.Count - (randomizedPopulation.Count % 2);

            for (int i = 0; i < randomizedPopulationCount; i+=2)
            {
                var scheme = Cross(randomizedPopulation[i], randomizedPopulation[i + 1]);
                Population.Add(scheme);
            }
        }

        // Функция соответствия модели схемы заданным параметрам
        public void Fit(FESchemeModel model)
        {
            var points = new List<(double frequency, double phase)>();

            int rate = 0;

            double frequency = MinFrequency;
            // цикл по частотам
            for (int i = 0; i < PointsCountAtFrequencyAxle; i++)
            {
                var phase = SchemePhaseResponseCalculator.CalculatePhase(model, frequency);

                // если точка попадает в окно
                if (phase > LowerCharacteristicBound & phase < UpperCharacteristicBound)
                {
                    rate++;
                }

                points.Add((frequency, phase));

                frequency += FrequencyIncrement;
            }

            model.StateInGA.Rate = rate;
            model.PhaseResponsePoints = points;
        }

        public void FitByStandardDeviation(FESchemeModel model)
        {
            var points = new List<(double frequency, double phase)>();

            double S = 0;

            double frequency = MinFrequency;
            // цикл по частотам
            for (int i = 0; i < PointsCountAtFrequencyAxle; i++)
            {
                var phase = SchemePhaseResponseCalculator.CalculatePhase(model, frequency);

                S += Math.Pow(phase - MinFrequency, 2);

                points.Add((frequency, phase));

                frequency += FrequencyIncrement;
            }

            double rate = Math.Sqrt(S / PointsCountAtFrequencyAxle);

            model.StateInGA.Rate = 1 / rate;
            model.PhaseResponsePoints = points;
        }


        // Метод для оценки популяции
        public void RatePopulation() 
        {
            foreach (var scheme in Population)
            {
                Fit(scheme.Model);            
            }
        }

        // Метод для отбора популяции
        public void SelectPopulation()
        {
            var newPopulation = Population.OrderByDescending(x => x.Model.StateInGA.Rate).Take(100).ToList();

            Population = newPopulation;
        }

        // Метод для получения популяции
        public List<FElementScheme> GetPopulation() 
        {
            return Population;
        }

        // Метод для старта алгоритма
        public void Start() 
        {
            InitiatePopulation(SchemePrototype);

            double increment = 100f / CountOfWholeStepsOfGA;

            for (int i = 0; i < CountOfWholeStepsOfGA; i++)
            {
                OnDoWork(increment * (i + 1));

                // для сравнения мутировавших особей
                //var t = ga.Population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();

                MutatePopulation();

                // для сравнения мутировавших особей
                //var t1 = ga.Population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();

                CrossPopulation();

                //var t2 = ga.Population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();

                RatePopulation();

                // для просмотра оценки 
                //var t3 = ga.Population.OrderByDescending(x => x.Model.Rate).Select(x => x.Model).ToList();

                SelectPopulation();

                //var t99 = population.Select(x => x.Model.InnerConnections.Select(y => y.ConnectionType).ToList()).ToList();
                //var t9 = population.Select(x => x.Model.InnerConnections.Select(y => y.PEType).ToList()).ToList();
            }
        }
    }
}
