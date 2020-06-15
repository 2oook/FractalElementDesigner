using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Elements
{
    /// <summary>
    /// Interaction logic for CellControl.xaml
    /// </summary>
    public partial class CellControl : UserControl
    {
        public CellControl()
        {
            InitializeComponent();
        }

        public CellControl(double height, double width) : this()
        {
            canvas.Width = width;
            canvas.Height = height;

            grid.Width = width;
            grid.Height = height;

            path.Data = Geometry.Parse("M 0,0 L 0," + height + " L " + width + "," + height + "L " + width + ",0 Z ");
        }

        /// <summary>
        /// Событие нажатия на контрол ячейки
        /// </summary>
        public event Action Click = delegate { };

        /// <summary>
        /// Обработчик нажатия на ячейку
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Объект параметров события</param>
        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Click();
        }
    }
}
