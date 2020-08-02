using MahApps.Metro.Controls;
using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// Схема включения элемента
    /// </summary>
    [Serializable]
    class FElementScheme : IFElementSchemePrototype, IProjectTreeItem
    {
        /// <summary>
        /// Допустимые подключения двух четырехполюсников
        /// </summary>
        public static Dictionary<int, AllowablePinsConnection> AllowablePinsConnections;

        /// <summary>
        /// Статический конструктор
        /// </summary>
        static FElementScheme()
        {
            // порядок обхода каждого элемента: левый верхний -> правый верхний -> правый нижний -> левый нижний
            AllowablePinsConnections = new Dictionary<int, AllowablePinsConnection>()
            {
                {
                    1,
                    new AllowablePinsConnection()
                    {
                        ConnectionMatrix = new int[,]
                        {
                            { 1, 1, 1, 1 },
                            { 1, 1, 1, 1 },
                            { 1, 1, 1, 1 },
                            { 1, 1, 1, 1 }
                        },
                        PEVector = new Dictionary<int, int[]>
                        {
                            {
                                1,
                                new int[] { 0,0,0,0 }
                            }
                        } 
                    } 
                },
                {
                    2,
                    new AllowablePinsConnection()
                    {
                        ConnectionMatrix = new int[,]
                        {
                            { 1, 1, 1, 0 },
                            { 1, 1, 1, 0 },
                            { 1, 1, 1, 0 },
                            { 0, 0, 0, 1 }
                        },
                        PEVector = new Dictionary<int, int[]>
                        {
                            {
                                1,
                                new int[] { 0,0,0,0 }
                            },
                            {
                                2,
                                new int[] { 0,0,0,1 }
                            }
                        }
                    } 
                },
                {
                    3,
                    new AllowablePinsConnection()
                    {
                        ConnectionMatrix = new int[,]
                        {
                            { 1, 1, 0, 1 },
                            { 1, 1, 0, 1 },
                            { 0, 0, 1, 0 },
                            { 1, 1, 0, 1 }
                        },
                        PEVector = new Dictionary<int, int[]>
                        {
                            {
                                1,
                                new int[] { 0,0,0,0 }
                            },
                            {
                                2,
                                new int[] { 0,0,1,0 }
                            }
                        }
                    }
                },
                {
                    4,
                    new AllowablePinsConnection()
                    {
                        ConnectionMatrix = new int[,]
                        {
                            { 1, 1, 0, 0 },
                            { 1, 1, 0, 0 },
                            { 0, 0, 1, 0 },
                            { 0, 0, 0, 1 }
                        },
                        PEVector = new Dictionary<int, int[]>
                        {
                            {
                                1,
                                new int[] { 0,0,0,0 }
                            },
                            {
                                2,
                                new int[] { 0,0,1,0 }
                            },
                            {
                                3,
                                new int[] { 0,0,0,1 }
                            },
                            {
                                4,
                                new int[] { 0,0,1,1 }
                            }
                        }
                    }
                },
                {
                    5,
                    new AllowablePinsConnection()
                    {
                        ConnectionMatrix = new int[,]
                        {
                            { 1, 0, 1, 1 },
                            { 0, 1, 0, 0 },
                            { 1, 0, 1, 1 },
                            { 1, 0, 1, 1 }
                        },
                        PEVector = new Dictionary<int, int[]>
                        {
                            {
                                1,
                                new int[] { 0,0,0,0 }
                            },
                            {
                                2,
                                new int[] { 0,1,0,0 }
                            }
                        }
                    }
                },
                {
                    6,
                    new AllowablePinsConnection()
                    {
                        ConnectionMatrix = new int[,]
                        {
                            { 1, 0, 0, 0 },
                            { 0, 1, 0, 0 },
                            { 0, 0, 1, 1 },
                            { 0, 0, 1, 1 }
                        },
                        PEVector = new Dictionary<int, int[]>
                        {
                            {
                                1,
                                new int[] { 0,0,0,0 }
                            },
                            {
                                2,
                                new int[] { 1,0,0,0 }
                            },
                            {
                                3,
                                new int[] { 0,1,0,0 }
                            },
                            {
                                4,
                                new int[] { 1,1,0,0 }
                            }
                        }
                    }
                },
                {
                    7,
                    new AllowablePinsConnection()
                    {
                        ConnectionMatrix = new int[,]
                        {
                            { 1, 1, 0, 0 },
                            { 1, 1, 0, 0 },
                            { 0, 0, 1, 1 },
                            { 0, 0, 1, 1 }
                        },
                        PEVector = new Dictionary<int, int[]>
                        {
                            {
                                1,
                                new int[] { 0,0,0,0 }
                            },
                            {
                                2,
                                new int[] { 1,1,0,0 }
                            },
                            {
                                3,
                                new int[] { 0,0,1,1 }
                            }
                        }
                    }     
                },
                {
                    8,
                    new AllowablePinsConnection()
                    {
                        ConnectionMatrix = new int[,]
                        {
                            { 1, 0, 0, 0 },
                            { 0, 1, 1, 1 },
                            { 0, 1, 1, 1 },
                            { 0, 1, 1, 1 }
                        },
                        PEVector = new Dictionary<int, int[]>
                        {
                            {
                                1,
                                new int[] { 0,0,0,0 }
                            },
                            {
                                2,
                                new int[] { 1,0,0,0 }
                            }
                        }
                    }   
                }
            };
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public FElementScheme(List<FESection> sections)
        {
            sections = sections.OrderBy(x => x.Number).ToList();

            FESections = sections;

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
                                PEType = 3,
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
                                PEType = 1,
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
                                PEType = 3,
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
        public List<Connection> InnerConnections { get; set; } = new List<Connection>();

        /// <summary>
        /// Вектор перестановки нумерации выводов схемы
        /// </summary>
        public int[] PinsNumbering { get; set; }

        /// <summary>
        /// Точки ФЧХ
        /// </summary>
        public List<(double frequency, double phase)> PhaseResponsePoints { get; set; }

        /// <summary>
        /// Элементы схемы
        /// </summary>
        public ObservableCollection<IProjectTreeItem> Elements { get; set; } = new ObservableCollection<IProjectTreeItem>();

        /// <summary>
        /// Название схемы
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

        // Метод для клонирования схемы
        public IFElementSchemePrototype DeepClone()
        {
            FElementScheme scheme;

            var formatter = new BinaryFormatter();         

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                scheme = formatter.Deserialize(stream) as FElementScheme;

                if (scheme == null)
                {
                    throw new Exception("Ошибка сериализации объекта схемы!");
                }
            }

            return scheme;
        }
    }
}
