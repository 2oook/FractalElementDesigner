using MahApps.Metro.Controls;
using FractalElementDesigner.ProjectTree;
using FractalElementDesigner.SchemeEditing.Editor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
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

            Model.FESections = sections;

            var array = new List<int>();

            for (int i = 0; i < sections.Count; i++)
            {
                array = array.Concat(sections[i].Pins.Select(x => x.Number)).ToList();
            }

            Model.PinsNumbering = array.ToArray();

            // инициализация внешних выводов
            for (int i = 1; i <= sections.First().Pins.Count; i++)
            {
                var outer_pin = new OuterPin() { Number = i };

                if (i == 1)
                {
                    outer_pin.State = OuterPinState.In;
                }
                else
                {
                    outer_pin.State = OuterPinState.Gnd;
                }

                Model.OuterPins.Add(outer_pin);
            }

            // тест!!!!!!!!!!!!!!!!!!!!!!!!
            // для схемы из матлаб
            foreach (var pin in Model.OuterPins)
            {
                if (pin.Number == 1)
                {
                    pin.State = OuterPinState.Gnd;
                }
                else if (pin.Number == 2)
                {
                    pin.State = OuterPinState.In;
                }
                else if (pin.Number == 3)
                {
                    pin.State = OuterPinState.Con;
                }
                else
                {
                    pin.State = OuterPinState.Con;
                }
            }
            // тест!!!!!!!!!!!!!!!!!!!!!!!!

            // инициализация соединений между секциями
            for (int i = 0; i < sections.Count - 1; i++)
            {
                //InnerConnections.Add(new Connection()
                //{
                //    FirstSection = sections[sections[i].Number - 1],
                //    SecondSection = sections[sections[i].Number + 1 - 1]
                //});

                // тест!!!!!!!!!!!!!!!!!!!!!!!!
                // для схемы из матлаб
                // инициализация 3-х соединений
                switch (i)
                {
                    case 0:
                        {
                            var type = 7;

                            Model.InnerConnections.Add(new Connection()
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

                            Model.InnerConnections.Add(new Connection()
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

                            Model.InnerConnections.Add(new Connection()
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
        }

        /// <summary>
        /// Модель схемы
        /// </summary>
        public FESchemeModel Model { get; set; } = new FESchemeModel();

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
        [NonSerialized]
        public bool IsLocked = false;

        [NonSerialized]
        private SchemeEditor editor;
        /// <summary>
        /// Объект редактора схемы
        /// </summary>
        public SchemeEditor Editor
        {
            get { return editor; }
            set { editor = value; }
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
