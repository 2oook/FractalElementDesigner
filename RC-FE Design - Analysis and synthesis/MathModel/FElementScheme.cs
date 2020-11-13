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
using FractalElementDesigner.StructureSchemeSynthesis;
using FractalElementDesigner.FEEditing.Model.StructureElements;

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
        /// Допустимые подключения в виде типа структуры и типов ячеек по слоям
        /// </summary>
        public static Dictionary<(int, int), List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>> AllowablePinsConnectionsOnLayer;

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

            AllowablePinsConnectionsOnLayer = new Dictionary<(int, int), List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>>()
            {
                {
                    (1, 1), 
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk))
                    }
                },
                {
                    (2, 1),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rv,  GetCellTypeList(StructureSegmentTypeEnum.Rv)),
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk))
                    }
                },
                {
                    (2, 2),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.R_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.R_C_NRk)),
                        (StructureSegmentTypeEnum.Rv, GetCellTypeList(StructureSegmentTypeEnum.Rv)),
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk))
                    }
                },
                {
                    (3, 1),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk)),
                        (StructureSegmentTypeEnum.Rv, GetCellTypeList(StructureSegmentTypeEnum.Rv))
                    }
                },
                {
                    (3, 2),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk)),
                        (StructureSegmentTypeEnum.Rv, GetCellTypeList(StructureSegmentTypeEnum.Rv)),
                        (StructureSegmentTypeEnum.R_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.R_C_NRk))
                    }
                },
                {
                    (4, 1),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rv, GetCellTypeList(StructureSegmentTypeEnum.Rv))
                    }
                },
                 {
                    (4, 2),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rv, GetCellTypeList(StructureSegmentTypeEnum.Rv)),
                        (StructureSegmentTypeEnum.R_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.R_C_NRk))
                    }
                },
                  {
                    (4, 3),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.R_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.R_C_NRk)),
                        (StructureSegmentTypeEnum.Rv, GetCellTypeList(StructureSegmentTypeEnum.Rv))
                    }
                },
                   {
                    (4, 4),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.R_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.R_C_NRk))
                    }
                },
                    {
                    (5, 1),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk)),
                        (StructureSegmentTypeEnum.Rn, GetCellTypeList(StructureSegmentTypeEnum.Rn))
                    }
                },
                     {
                    (5, 2),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk)),
                        (StructureSegmentTypeEnum.Rn, GetCellTypeList(StructureSegmentTypeEnum.Rn)),
                        (StructureSegmentTypeEnum.Rk_C_NR, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NR))
                    }
                },
                     {
                    (6, 1),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rn, GetCellTypeList(StructureSegmentTypeEnum.Rn))
                    }
                },
                 {
                    (6, 2),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NR, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NR)),
                        (StructureSegmentTypeEnum.Rn, GetCellTypeList(StructureSegmentTypeEnum.Rn))
                    }
                },
                  {
                    (6, 3),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rn, GetCellTypeList(StructureSegmentTypeEnum.Rn)),
                        (StructureSegmentTypeEnum.Rk_C_NR, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NR))
                    }
                },
                   {
                    (6, 4),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NR, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NR))
                    }
                },
{
                    (7, 1),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.R_C_NR, GetCellTypeList(StructureSegmentTypeEnum.R_C_NR))
                    }
                },
                 {
                    (7, 2),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NR, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NR))
                    }
                },
                  {
                    (7, 3),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.R_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.R_C_NRk))
                    }
                },
                  {
                    (8, 1),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rn, GetCellTypeList(StructureSegmentTypeEnum.Rn)),
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk))
                    }
                },
                 {
                    (8, 2),
                    new List<(StructureSegmentTypeEnum structureSegmentsTypes, List<CellType> cellsTypes)>()
                    {
                        (StructureSegmentTypeEnum.Rk_C_NR, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NR)),
                        (StructureSegmentTypeEnum.Rn, GetCellTypeList(StructureSegmentTypeEnum.Rn)),
                        (StructureSegmentTypeEnum.Rk_C_NRk, GetCellTypeList(StructureSegmentTypeEnum.Rk_C_NRk))
                    }
                }
            };

            List<CellType> GetCellTypeList(StructureSegmentTypeEnum structureSegmentType) 
            {
                switch (structureSegmentType)
                {
                    case StructureSegmentTypeEnum.R_C_NR:
                        return new List<CellType>() { CellType.RC, CellType.R };
                    case StructureSegmentTypeEnum.Rv:
                        return new List<CellType>() { CellType.RC, CellType.Cut };
                    case StructureSegmentTypeEnum.Rn:
                        return new List<CellType>() { CellType.Cut, CellType.R };
                    case StructureSegmentTypeEnum.Rk_C_NRk:
                        return new List<CellType>() { CellType.Rk, CellType.NRk };
                    case StructureSegmentTypeEnum.R_C_NRk:
                        return new List<CellType>() { CellType.RC, CellType.NRk };
                    case StructureSegmentTypeEnum.Rk_C_NR:
                        return new List<CellType>() { CellType.Rk, CellType.R };
                    default:
                        return new List<CellType>() { CellType.RC, CellType.R };
                }
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public FElementScheme(StructureSchemeSynthesisParameters synthesisParameters)
        {
            SynthesisParameters = synthesisParameters;

            var sections = synthesisParameters.FESections;

            sections = sections.OrderBy(x => x.Number).ToList();

            Model.FESections = sections;

            var array = new List<int>();

            for (int i = 0; i < sections.Count; i++)
            {
                array = array.Concat(sections[i].Pins.Select(x => x.Number)).ToList();
            }

            Model.PinsNumbering = array.ToArray();

            // инициализация внешних выводов
            for (int i = 0; i < sections.First().Pins.Count; i++)
            {
                var outer_pin = new OuterPin();

                if (i == 0)
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
            for (int i = 0; i < Model.OuterPins.Count; i++)
            {
                var pin = Model.OuterPins[i];

                if (i == 0)
                {
                    pin.State = OuterPinState.Gnd;
                }
                else if (i == 1)
                {
                    pin.State = OuterPinState.In;
                }
                else if (i == 2)
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
        /// Параметры
        /// </summary>
        public StructureSchemeSynthesisParameters SynthesisParameters;

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
