using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// ИНтерфейс представляет прототип схемы
    /// </summary>
    interface IFElementSchemePrototype
    {
        /// <summary>
        /// Метод для рекурсивного клонирования объекта схемы
        /// </summary>
        /// <returns>Объект схемы</returns>
        IFElementSchemePrototype DeepClone();
    }
}
