using FractalElementDesigner.FEEditing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// Отображение вывода на двухмерную реализацию
    /// </summary>
    class LayerConnectionItem
    {
        /// <summary>
        /// Слой структуры
        /// </summary>
        internal Layer Layer = null;

        /// <summary>
        /// Присутствует ли заземление
        /// </summary>
        internal bool Grounded = false;

        /// <summary>
        /// Список соединений с другими элементами
        /// </summary>
        internal List<LayerConnectionItem> Connection = new List<LayerConnectionItem>();
    }
}
