using FractalElementDesigner.FEEditing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FractalElementDesigner.FEEditing.Controls
{
    /// <summary>
    /// Элемент для редактирования структуры
    /// </summary>
    public class FECanvas : Canvas, ICanvas
    {
        /// <summary>
        /// Изначальная высота элемента 
        /// </summary>
        public double InitialHeight { get; set; } = 0;

        /// <summary>
        /// Изначальная ширина элемента
        /// </summary>
        public double InitialWidth { get; set; } = 0;

        public void Add(IElement element)
        {
            if (element != null)
            {
                this.Children.Add(element as FrameworkElement);
            }
        }

        public void Remove(IElement element)
        {
            if (element != null)
            {
                this.Children.Remove(element as FrameworkElement);
            }
        }

        public void Clear()
        {
            this.Children.Clear();
        }

        public IEnumerable<IElement> GetElements()
        {
            return this.Children.Cast<FrameworkElement>().Cast<IElement>();
        }

        public double GetWidth()
        {
            return this.Width;
        }

        public void SetWidth(double width)
        {
            this.Width = width;
        }

        public double GetHeight()
        {
            return this.Height;
        }

        public void SetHeight(double height)
        {
            this.Height = height;
        }
    }
}
