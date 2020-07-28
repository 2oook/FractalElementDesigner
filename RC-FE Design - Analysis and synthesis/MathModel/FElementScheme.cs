using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Схема включения элемента
    /// </summary>
    public class FElementScheme : IProjectTreeItem
    {
        /// <summary>
        /// Матрицы инцидентности допустимых подключений двух четырехполюсников
        /// </summary>
        public static Dictionary<int, int[,]> IncidenceMatrices_E;

        /// <summary>
        /// Матрицы указывающие на заземлённые выводы
        /// </summary>
        public static List<int[]> GroundVectors_A;

        /// <summary>
        /// Коды допустимых заземлений четырёхполюсника
        /// </summary>
        public static Dictionary<int, int[]> GroundCodes_A;

        /// <summary>
        /// Статический конструктор
        /// </summary>
        static FElementScheme()
        {
            // порядок обхода каждого элемента: левый верхний -> правый верхний -> правый нижний -> левый нижний
            IncidenceMatrices_E = new Dictionary<int, int[,]>()
            {
                {
                    1,
                    new int[,]
                    {
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 }
                    }
                },
                {
                    2,
                    new int[,]
                    {
                        { 1, 1, 1, 0 },
                        { 1, 1, 1, 0 },
                        { 1, 1, 1, 0 },
                        { 0, 0, 0, 1 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 1, 1, 0, 1 },
                        { 1, 1, 0, 1 },
                        { 0, 0, 1, 0 },
                        { 1, 1, 0, 1 }
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 1, 1, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 1, 0 },
                        { 0, 0, 0, 1 }
                    }
                },
                {
                    5,
                    new int[,]
                    {
                        { 1, 0, 1, 1 },
                        { 0, 1, 0, 0 },
                        { 1, 0, 1, 1 },
                        { 1, 0, 1, 1 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 1, 0, 0, 0 },
                        { 0, 1, 0, 0 },
                        { 0, 0, 1, 1 },
                        { 0, 0, 1, 1 }
                    }
                },
                {
                    7,
                    new int[,]
                    {
                        { 1, 1, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 1, 1 },
                        { 0, 0, 1, 1 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 1, 0, 0, 0 },
                        { 0, 1, 1, 1 },
                        { 0, 1, 1, 1 },
                        { 0, 1, 1, 1 }
                    } 
                }
            };

            GroundVectors_A = new List<int[]>()
            {
                new int[] { 1, 0, 0, 0 },
                new int[] { 0, 1, 0, 0 },
                new int[] { 0, 0, 1, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 1, 1, 0, 0 },
                new int[] { 0, 0, 1, 1 }
            };

            GroundCodes_A = new Dictionary<int, int[]>()
            {
                {
                    1,
                    new int[] {  }
                },
                {
                    2,
                    new int[] { 0 }
                },
                {
                    3,
                    new int[] { 1 }
                },
                {
                    4,
                    new int[] { 2 }
                },
                {
                    5,
                    new int[] { 3 }
                },
                {
                    6,
                    new int[] { 0, 1 }
                },
                {
                    7,
                    new int[] { 2, 3 }
                },
            };
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public FElementScheme(List<FESection> sections)
        {
            sections = sections.OrderBy(x => x.Number).ToList();

            FESections = sections;

            InnerConnections = new List<Connection>();

            var array = new List<int>();

            for (int i = 0; i < sections.Count; i++)
            {
                array = array.Concat(sections[i].Pins.Select(x => x.Number)).ToList();//!!!!!!!!!!!!!!!!!!!!
            }
            
            for (int i = 0; i < sections.Count - 1; i++)
            {
                //InnerConnections.Add(new Connection()
                //{
                //    FirstSection = sections[sections[i].Number - 1],
                //    SecondSection = sections[sections[i].Number + 1 - 1]
                //});

                // тест!!!!!!!!!!!!!!!!!!!!!!!!
                // инициализация 3-х соединений
                switch (i)
                {
                    case 0:
                        {
                            var type = 7;

                            InnerConnections.Add(new Connection()
                            {
                                ConnectionType = type,
                                FirstSection = sections[sections[i].Number - 1],
                                SecondSection = sections[sections[i].Number + 1 - 1]
                            });
                        }
                        break;
                    case 1:
                        {
                            var type = 8; 
                            
                            InnerConnections.Add(new Connection()
                            {
                                ConnectionType = type,
                                FirstSection = sections[sections[i].Number - 1],
                                SecondSection = sections[sections[i].Number + 1 - 1]
                            });
                        }
                        break;
                    case 2:
                        {
                            var type = 4; 
                            
                            InnerConnections.Add(new Connection()
                            {
                                ConnectionType = type,
                                FirstSection = sections[sections[i].Number - 1],
                                SecondSection = sections[sections[i].Number + 1 - 1]
                            });
                        }
                        break;
                }
            }
            // тест!!!!!!!!!!!!!!!!!!!!!!!!

            PinsNumbering = array.ToArray();
        }

        /// <summary>
        /// Секции ФРЭ
        /// </summary>
        public List<FESection> FESections { get; set; }

        /// <summary>
        /// Соединения БКЭ
        /// </summary>
        public List<Connection> InnerConnections { get; set; }

        /// <summary>
        /// Вектор перестановки нумерации выводов схемы
        /// </summary>
        public int[] PinsNumbering { get; set; }

        /// <summary>
        /// Графики
        /// </summary>
        public ObservableCollection<PRPlot> Plots { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Заблокирована ли схема
        /// </summary>
        public bool IsLocked { get; set; } = false;

        // Метод для очистки состояния схемы 
        public bool ClearState() 
        {
            bool result = true;

            return result;
        }
    }
}
