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
        public static List<List<int[][]>> IncidenceMatrices_E;

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
            IncidenceMatrices_E = new List<List<int[][]>>()
            {
                new List<int[][]>
                {
                    new int[][]
                    { 
                        new int[] { 1, 1, 1, 1 },
                        new int[] { 1, 1, 1, 1 },
                        new int[] { 1, 1, 1, 1 },
                        new int[] { 1, 1, 1, 1 }
                    },
                    new int[][]
                    {
                        new int[] { 1, 1, 0, 0 },
                        new int[] { 1, 1, 1, 0 },
                        new int[] { 0, 1, 1, 0 },
                        new int[] { 0, 0, 0, 1 }
                    },
                    new int[][]
                    {
                        new int[] { 1, 1, 0, 1 },
                        new int[] { 1, 1, 0, 0 },
                        new int[] { 0, 0, 1, 0 },
                        new int[] { 1, 0, 0, 1 }
                    },
                    new int[][]
                    {
                        new int[] { 1, 1, 0, 0 },
                        new int[] { 1, 1, 0, 0 },
                        new int[] { 0, 0, 1, 0 },
                        new int[] { 0, 0, 0, 1 }
                    }
                },
                new List<int[][]>
                {
                    new int[][]
                    {
                        new int[] { 1, 0, 0, 1 },
                        new int[] { 0, 1, 0, 0 },
                        new int[] { 0, 0, 1, 1 },
                        new int[] { 1, 0, 1, 1 }
                    },
                    new int[][]
                    {
                        new int[] { 1, 0, 0, 0 },
                        new int[] { 0, 1, 0, 0 },
                        new int[] { 0, 0, 1, 1 },
                        new int[] { 0, 0, 1, 1 }
                    },
                    new int[][]
                    {
                        new int[] { 1, 1, 0, 0 },
                        new int[] { 1, 1, 0, 0 },
                        new int[] { 0, 0, 1, 1 },
                        new int[] { 0, 0, 1, 1 }
                    },
                    new int[][]
                    {
                        new int[] { 1, 0, 0, 0 },
                        new int[] { 0, 1, 1, 0 },
                        new int[] { 0, 1, 1, 1 },
                        new int[] { 0, 0, 1, 1 }
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

            var array = new List<int>();

            foreach (var section in sections)     
                array = array.Concat(section.SectionParameters.PinsSchemeNumeration).ToList();

            PinsNumbering = array.ToArray();
        }

        /// <summary>
        /// Секции ФРЭ
        /// </summary>
        public List<FESection> FESections { get; set; }

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
