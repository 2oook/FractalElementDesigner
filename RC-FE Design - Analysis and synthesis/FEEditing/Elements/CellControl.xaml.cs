using FractalElementDesigner.FEEditing.Model.StructureElements;
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

namespace FractalElementDesigner.FEEditing.Elements
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
    }
}
