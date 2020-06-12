using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Controls
{
    public class FECanvas : Canvas
    {
        public double InitialHeight { get; set; } = 0;

        public double InitialWidth { get; set; } = 0;

        public void Add(FrameworkElement element)
        {
            if (element != null) this.Children.Add(element as FrameworkElement);
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
