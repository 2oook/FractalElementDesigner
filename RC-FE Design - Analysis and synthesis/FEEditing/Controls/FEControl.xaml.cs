using FractalElementDesigner.FEEditing.Controls;
using FractalElementDesigner.FEEditing.Core;
using FractalElementDesigner.FEEditing.Interfaces;
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

namespace FractalElementDesigner.FEEditing
{
    /// <summary>
    /// Interaction logic for FEControl.xaml
    /// </summary>
    public partial class FEControl : UserControl
    {
        public FEControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Метод для создания области проектирования
        /// </summary>
        /// <returns></returns>
        public ICanvas CreateFECanvas()
        {
            var canvas = new FECanvas() { Name = "FECanvas", Background = (SolidColorBrush)Application.Current.Resources["LogicTransparentColorKey"] };

            canvas.MouseLeftButtonDown += FECanvas_MouseLeftButtonDown;
            canvas.MouseLeftButtonUp += FECanvas_MouseLeftButtonUp;
            canvas.PreviewMouseLeftButtonDown += FECanvas_PreviewMouseLeftButtonDown;
            canvas.MouseMove += FECanvas_MouseMove;
            canvas.PreviewMouseRightButtonDown += FECanvas_PreviewMouseRightButtonDown;
            canvas.ContextMenuOpening += FECanvas_ContextMenuOpening;

            canvas.Width = 1260;
            canvas.Height = 890;

            Grid.SetColumn(canvas, 0);
            Grid.SetRow(canvas, 0);

            var tg = new TransformGroup() { Children = { new ScaleTransform() { ScaleX = 1, ScaleY = 1 }, new SkewTransform(), new RotateTransform(), new TranslateTransform() } };
            RootGrid.RenderTransform = tg;

            return canvas;
        }

        #region Properties

        private Editor editor;
        /// <summary>
        /// Редактор
        /// </summary>
        public Editor Editor
        { 
            get => editor;
            set 
            {
                // если объект редактора не null очистить область редактирования и расположить в ней текущий редактор
                if (editor != null)
                {
                    RootGrid.Children.Clear();
                    RootGrid.Children.Add(value.Context.CurrentCanvas as FECanvas);
                }

                editor = value;
            }
        }

        private SelectionAdorner Adorner { get; set; }

        #endregion

        #region Fields

        public double DefaultLogicStrokeThickness = 1.0;
        public double DefaultWireStrokeThickness = 2.0;
        public double DefaultElementStrokeThickness = 2.0;
        public double DefaultIOStrokeThickness = 2.0;
        public double DefaultPageStrokeThickness = 1.0;
        private double CurrentZoom = 1.0;

        #endregion

        #region Zoom

        public void ResetZoom()
        {
            var st = GetZoomScaleTransform();
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;
        }

        private void UpdateStrokeThickness(double zoom)
        {
            Application.Current.Resources[ResourceConstants.KeyLogicStrokeThickness] = DefaultLogicStrokeThickness / zoom;
            Application.Current.Resources[ResourceConstants.KeyWireStrokeThickness] = DefaultWireStrokeThickness / zoom;
            Application.Current.Resources[ResourceConstants.KeyElementStrokeThickness] = DefaultElementStrokeThickness / zoom;
            Application.Current.Resources[ResourceConstants.KeyIOStrokeThickness] = DefaultIOStrokeThickness / zoom;
            Application.Current.Resources[ResourceConstants.KeyPageStrokeThickness] = DefaultPageStrokeThickness / zoom;
        }

        public double CalculateZoom(double x)
        {
            double lb = 1.9;
            double ef = 1.3;
            double l = (lb == 1.0 || lb == 0.0) ? 1.0 : Math.Log(x, lb);
            double e = (ef == 0.0) ? 1.0 : Math.Exp(l / ef);
            return x + x * l * e;
        }

        public void Zoom(double zoom)
        {
            if (Editor == null || Editor.Context == null)
                return;

            double czoom = CalculateZoom(zoom);
            var st = GetZoomScaleTransform();
            double old = st.ScaleX;

            st.ScaleX = czoom;
            st.ScaleY = czoom;

            UpdateStrokeThickness(czoom);
            ZoomToPoint(czoom, old);
        }

        private ScaleTransform GetZoomScaleTransform()
        {
            var tg = RootGrid.RenderTransform as TransformGroup;
            return tg.Children.First(t => t is ScaleTransform) as ScaleTransform;
        }

        private TranslateTransform GetZoomTranslateTransform()
        {
            var tg = RootGrid.RenderTransform as TransformGroup;
            return tg.Children.First(t => t is TranslateTransform) as TranslateTransform;
        }

        private void ZoomToPoint(double zoom, double oldZoom)
        {
            var relative = Editor.Context.ZoomPoint;
            var tt = GetZoomTranslateTransform();

            double absoluteX = relative.X * oldZoom + tt.X;
            double absoluteY = relative.Y * oldZoom + tt.Y;

            tt.X = absoluteX - relative.X * zoom;
            tt.Y = absoluteY - relative.Y * zoom;

            if (Adorner != null) Adorner.Zoom = zoom;
        }

        private void ZoomToFit(Size viewport, Size source)
        {
            var st = GetZoomScaleTransform();
            var tt = GetZoomTranslateTransform();
            double sX = viewport.Width / source.Width;
            double sY = viewport.Height / source.Height;
            double zoom = Math.Min(sX, sY);
            double dXZoomed = viewport.Width - (source.Width * zoom);
            double dYZoomed = viewport.Height - (source.Height * zoom);
            double dX = viewport.Width - source.Width;
            double dY = viewport.Height - source.Height;
            double x = 0.0;
            double y = 0.0;

            if (source.Width >= viewport.Width && source.Height >= viewport.Height)
            {
                x = sX > sY ? dXZoomed / 2.0 : 0.0;
                y = sX > sY ? 0.0 : dYZoomed / 2.0;
            }
            else
            {
                x = source.Width >= viewport.Width ? 0.0 : (dXZoomed - dX) / 2.0;
                y = source.Height >= viewport.Height ? 0.0 : (dYZoomed - dY) / 2.0;
            }

            st.ScaleX = zoom;
            st.ScaleY = zoom;
            tt.X = x;
            tt.Y = y;

            UpdateStrokeThickness(zoom);

            if (Adorner != null) Adorner.Zoom = zoom;

            CurrentZoom = 1.0;
        }

        public void ZoomToFit()
        {
            var viewport = new Size(this.ActualWidth + 0.0, this.ActualHeight + 0.0);
            var source = new Size(this.Editor.Context.CurrentCanvas.GetWidth() + 6.0, this.Editor.Context.CurrentCanvas.GetHeight() + 6.0);
            ZoomToFit(viewport, source);
        }

        private void ZoomIn()
        {
            double zoom = CurrentZoom + Editor.Context.ZoomInFactor;

            if (zoom >= 0.1 && zoom <= 5.0)
            {
                CurrentZoom = zoom;
                Zoom(zoom);
            }
        }

        private void ZoomOut()
        {
            double zoom = CurrentZoom - Editor.Context.ZoomOutFactor;

            if (zoom >= 0.1 && zoom <= 5.0)
            {
                CurrentZoom = zoom;
                Zoom(zoom);
            }
        }

        #endregion

        #region Pan

        public void ResetPan()
        {
            var st = GetZoomTranslateTransform();
            st.X = 0.0;
            st.Y = 0.0;
        }

        private void BeginPan(Point point)
        {
            Editor.Context.PanStart = new PointEx(point.X, point.Y);
            Editor.Context.PreviousScrollOffsetX = -1.0;
            Editor.Context.PreviousScrollOffsetY = -1.0;

            this.Cursor = Cursors.ScrollAll;
            this.CaptureMouse();
        }

        private void EndPan()
        {
            if (this.IsMouseCaptured == true)
            {
                this.Cursor = Cursors.Arrow;
                this.ReleaseMouseCapture();
            }
        }

        private void PanToPoint(Point point)
        {
            double dX = point.X - Editor.Context.PanStart.X;
            double dY = point.Y - Editor.Context.PanStart.Y;
            var st = GetZoomTranslateTransform();
            st.X += dX;
            st.Y += dY;
            Editor.Context.PanStart = new PointEx(point.X, point.Y);
        }

        #endregion

        #region UserControl Events

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 1)
                BeginPan(e.GetPosition(this));
            else if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 2)
                ZoomToFit();

            this.Focus();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured == true)
                PanToPoint(e.GetPosition(this));
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle) EndPan();
        }

        #endregion

        private void RootBorder_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift) return;

            var canvas = Editor.Context.CurrentCanvas;
            var point = e.GetPosition(canvas as FECanvas);
            Editor.Context.ZoomPoint = new PointEx(point.X, point.Y);

            if (e.Delta > 0)
            {
                ZoomIn();
                e.Handled = true;
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
                e.Handled = true;
            }
        }

        private void FECanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void FECanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void FECanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void FECanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void FECanvas_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void FECanvas_Drop(object sender, DragEventArgs e)
        {

        }

        private void FECanvas_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void FECanvas_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }
    }
}
