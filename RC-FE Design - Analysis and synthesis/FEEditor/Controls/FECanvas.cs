using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FractalElementDesigner.FEEditor.Controls
{
    /// <summary>
    /// Элемент для редактирования структуры
    /// </summary>
    [Serializable]
    public class FECanvas : Canvas
    {
        /// <summary>
        /// Изначальная высота элемента 
        /// </summary>
        public double InitialHeight { get; set; } = 0;

        /// <summary>
        /// Изначальная ширина элемента
        /// </summary>
        public double InitialWidth { get; set; } = 0;
    }
}
