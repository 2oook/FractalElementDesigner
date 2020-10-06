using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Класс представляет модель схемы 
    /// </summary>
    [Serializable]
    class FESchemeModel : IFESchemeModelPrototype
    {
        /// <summary>
        /// Секции ФРЭ
        /// </summary>
        public List<FESection> FESections { get; set; }

        /// <summary>
        /// Соединения БКЭ
        /// </summary>
        public List<Connection> InnerConnections { get; set; } = new List<Connection>();

        /// <summary>
        /// Внешние выводы
        /// </summary>
        public List<OuterPin> OuterPins { get; set; } = new List<OuterPin>();

        /// <summary>
        /// Вектор перестановки нумерации выводов схемы
        /// </summary>
        public int[] PinsNumbering { get; set; }

        /// <summary>
        /// Информация об особи
        /// </summary>
        public StateOfIndividual StateInGA { get; set; } = new StateOfIndividual();

        /// <summary>
        /// Точки функции ФЧХ
        /// </summary>
        [NonSerialized]
        public List<(double frequency, double phase)> PhaseResponsePoints;

        // Метод для клонирования модели схемы
        public IFESchemeModelPrototype DeepClone()
        {
            FESchemeModel scheme;

            var formatter = new BinaryFormatter();

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                scheme = formatter.Deserialize(stream) as FESchemeModel;

                if (scheme == null)
                {
                    throw new Exception("Ошибка сериализации объекта модели схемы!");
                }
            }

            return scheme;
        }
    }
}
