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
        public static List<List<int[,]>> IncidenceMatrices_E;

        /// <summary>
        /// Матрицы указывающие на заземлённые выводы
        /// </summary>
        public static List<int[]> GroundVectors_A;

        /// <summary>
        /// Статический конструктор
        /// </summary>
        static FElementScheme()
        {
            // порядок обхода каждого элемента: левый верхний -> правый верхний -> правый нижний -> левый нижний
            IncidenceMatrices_E = new List<List<int[,]>>()
            {
                new List<int[,]>
                {
                    new int[,]
                    { 
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 }
                    },
                    new int[,]
                    {
                        { 1, 1, 0, 0 },
                        { 1, 1, 1, 0 },
                        { 0, 1, 1, 0 },
                        { 0, 0, 0, 1 }
                    },
                    new int[,]
                    {
                        { 1, 1, 0, 1 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 1, 0 },
                        { 1, 0, 0, 1 }
                    },
                    new int[,]
                    {
                        { 1, 1, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 1, 0 },
                        { 0, 0, 0, 1 }
                    }
                },
                new List<int[,]>
                {
                    new int[,]
                    {
                        { 1, 0, 0, 1 },
                        { 0, 1, 0, 0 },
                        { 0, 0, 1, 1 },
                        { 1, 0, 1, 1 }
                    },
                    new int[,]
                    {
                        { 1, 0, 0, 0 },
                        { 0, 1, 0, 0 },
                        { 0, 0, 1, 1 },
                        { 0, 0, 1, 1 }
                    },
                    new int[,]
                    {
                        { 1, 1, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 1, 1 },
                        { 0, 0, 1, 1 }
                    },
                    new int[,]
                    {
                        { 1, 0, 0, 0 },
                        { 0, 1, 1, 0 },
                        { 0, 1, 1, 1 },
                        { 0, 0, 1, 1 }
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
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public FElementScheme(List<FESection> sections)
        {
            FESections = sections;

            InnerConnections = new List<Connection>();

            var array = new List<int>();

            for (int i = 0; i < sections.Count; i++)
            {
                array = array.Concat(sections[i].SectionParameters.PinsSchemeNumeration).ToList();

                if (i < sections.Count - 1)
                {
                    // добавить соединение в котором участвуют текущая секция и следующая
                    InnerConnections.Add(new Connection()
                    {
                        Sections = new List<FESection>() 
                        {
                            sections[i],
                            sections[i + 1]
                        }
                    });
                }
            }

            // тест!!!!!!!!!!!!!!!!!!!!!!!!!!!
            InnerConnections[0].SchemeIndices = new [] { 1, 2 };
            InnerConnections[1].SchemeIndices = new [] { 1, 3 };
            InnerConnections[2].SchemeIndices = new [] { 0, 3 };
            // тест!!!!!!!!!!!!!!!!!!!!!!!!!!!

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
