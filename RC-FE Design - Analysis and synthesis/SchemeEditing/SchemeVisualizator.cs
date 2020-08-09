using FractalElementDesigner.MathModel;
using FractalElementDesigner.SchemeEditing.Controls;
using FractalElementDesigner.SchemeEditing.Core;
using FractalElementDesigner.SchemeEditing.Editor;
using FractalElementDesigner.SchemeEditing.Elements;
using FractalElementDesigner.SchemeEditing.Views;
using MahApps.Metro.Controls;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FractalElementDesigner.SchemeEditing
{
    class SchemeVisualizator
    {
        public static void InsertVisual(FElementScheme scheme)
        {
            var elementHeight = (double)Application.Current.FindResource("FEElementHeightKey");
            var elementWidth = (double)Application.Current.FindResource("FEElementWidthKey");

            // расположить элементы равномерно по ширине и высоте листа 
            var sheetHeight = scheme.Editor.Context.CurrentCanvas.GetHeight();
            var startVerticalAxeCoordinate = sheetHeight / 2;
            var startVerticalCoordinate = startVerticalAxeCoordinate - (elementHeight / 2);

            var sheetWidth = scheme.Editor.Context.CurrentCanvas.GetWidth();
            var horizontalPartOfWidth = sheetWidth / scheme.Model.FESections.Count;
            var startHorizontalAxeCoordinate = horizontalPartOfWidth / 2;
            var startHorizontalCoordinate = startHorizontalAxeCoordinate - (elementWidth/2);

            // отобразить секции
            for (int i = 0; i < scheme.Model.FESections.Count; i++)
            {
                var horizontalCoordinate = startHorizontalCoordinate + horizontalPartOfWidth * i;
                var addedElement = scheme.Editor.Add(scheme.Editor.Context.CurrentCanvas, Constants.TagElementFElement, new PointEx(horizontalCoordinate, startVerticalCoordinate));
               
                
                var t = addedElement as ElementThumb;

                var d = t.Template.VisualTree.FirstChild;


                var f = t.FindChildren<Canvas>();

                var t1 = addedElement as Control;
                var t2 = t1 as FElementControl;
                var y = t1.FindChildren<Canvas>();
            }

            //scheme.Editor.Context.CurrentCanvas.

            //var sections = scheme.Editor.Context.CurrentCanvas.GetChildren().OfType<Canvas>();

            //scheme.Editor.MouseEventPreviewLeftDown(scheme.Editor.Context.CurrentCanvas, , addedElement.);
        }
    }
}
