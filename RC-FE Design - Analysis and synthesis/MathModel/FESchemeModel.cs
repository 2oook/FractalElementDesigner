﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
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
        /// Вектор перестановки нумерации выводов схемы
        /// </summary>
        public int[] PinsNumbering { get; set; }

        /// <summary>
        /// Информация об особи
        /// </summary>
        public Individual Individual { get; set; }

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
