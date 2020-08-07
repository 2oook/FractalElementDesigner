using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// ИНтерфейс представляет прототип модели схемы
    /// </summary>
    interface IFESchemeModelPrototype
    {
        /// <summary>
        /// Метод для рекурсивного клонирования объекта модели схемы
        /// </summary>
        /// <returns>Объект модели схемы</returns>
        IFESchemeModelPrototype DeepClone();
    }
}
