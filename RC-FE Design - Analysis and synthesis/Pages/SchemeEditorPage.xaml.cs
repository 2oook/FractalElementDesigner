using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Controls;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Editor;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RC_FE_Design___Analysis_and_synthesis.Pages
{
    /// <summary>
    /// Interaction logic for SchemeEditorPage.xaml
    /// </summary>
    public partial class SchemeEditorPage : Page
    {
        #region Fields

        public SchemeEditor.Editor.SchemeEditor Editor { get; private set; }

        private PointEx InsertPointGate = new PointEx(325.0, 30.0);

        #endregion

        #region Constructor

        public SchemeEditorPage()
        {
            InitializeComponent();

            InitializeEditor();
            InitializeSchemeControl();
            InitializeWindowEvents();
            InitializeEditMenuEvents();
        }

        #endregion

        #region Initialize

        private void InitializeEditMenuEvents()
        {
            EditDelete.Click += (sender, e) => Delete();
            EditSelectAll.Click += (sender, e) => Editor.SelectAll();
            EditDeselectAll.Click += (sender, e) => DeselectAll();
            EditClear.Click += (sender, e) => Editor.ClearCanvas();
            EditConnect.Click += (sender, e) => Connect();
        }

        private void InitializeSchemeControl()
        {
            this.SchemeControl.Editor = this.Editor;
        }

        private void InitializeWindowEvents()
        {
            this.Loaded += (sender, e) =>
            {
                this.SchemeControl.Focus();
            };

            this.PreviewKeyDown += (sender, e) =>
            {
                if (!(e.OriginalSource is TextBox) &&
                    Keyboard.Modifiers != ModifierKeys.Shift)
                {
                    HandleKeyEvents(e);
                }
            };
        }

        private void InitializeEditor()
        {
            Editor = new SchemeEditor.Editor.SchemeEditor();
            Editor.Context = new Context();
            Editor.Context.CurrentCanvas = this.SchemeControl.SchemeCanvas;

            var counter = new IdCounter();
            counter.Set(3);
            this.SchemeControl.SchemeCanvas.SetCounter(counter);

            var prop = SchemeProperties.Default;
            this.SchemeControl.SchemeCanvas.SetProperties(prop);
            SetProperties(prop);

            Editor.Context.IsControlPressed = () => Keyboard.Modifiers == ModifierKeys.Control;
            Editor.Context.UpdateProperties = () => UpdateProperties(Editor.Context.CurrentCanvas.GetProperties());
            Editor.Context.SetProperties = (p) => SetProperties(p);

            // Scheme creator
            Editor.Context.SchemeCreator = GetSchemeCreator();

            // set checkbox states
            EnableSnap.IsChecked = Editor.Context.EnableSnap;
            SnapOnRelease.IsChecked = Editor.Context.SnapOnRelease;
        }

        private ISchemeCreator GetSchemeCreator()
        {
            var creator = new WpfSchemeCreator();

            creator.SetThumbEvents = (thumb) => SetThumbEvents(thumb);
            creator.SetPosition = (element, left, top, snap) => Editor.SetPosition(element, left, top, snap);

            creator.GetCounter = () => Editor.Context.CurrentCanvas.GetCounter();
            creator.SetCanvas(this.SchemeControl.SchemeCanvas);


            return creator;
        }

        private void SetThumbEvents(ElementThumb thumb)
        {
            thumb.DragDelta += (sender, e) =>
            {
                var canvas = Editor.Context.CurrentCanvas;
                var element = sender as IThumb;

                double dX = e.HorizontalChange;
                double dY = e.VerticalChange;

                Editor.Drag(canvas, element, dX, dY);
            };

            thumb.DragStarted += (sender, e) =>
            {
                var canvas = Editor.Context.CurrentCanvas;
                var element = sender as IThumb;

                Editor.DragStart(canvas, element);
            };

            thumb.DragCompleted += (sender, e) =>
            {
                var canvas = Editor.Context.CurrentCanvas;
                var element = sender as IThumb;

                Editor.DragEnd(canvas, element);
            };
        }

        private void UpdateProperties(SchemeProperties prop)
        {
            prop.PageWidth = int.Parse(TextPageWidth.Text);
            prop.PageHeight = int.Parse(TextPageHeight.Text);
            prop.GridOriginX = int.Parse(TextGridOriginX.Text);
            prop.GridOriginY = int.Parse(TextGridOriginY.Text);
            prop.GridWidth = int.Parse(TextGridWidth.Text);
            prop.GridHeight = int.Parse(TextGridHeight.Text);
            prop.GridSize = int.Parse(TextGridSize.Text);
            prop.SnapX = double.Parse(TextSnapX.Text);
            prop.SnapY = double.Parse(TextSnapY.Text);
            prop.SnapOffsetX = double.Parse(TextSnapOffsetX.Text);
            prop.SnapOffsetY = double.Parse(TextSnapOffsetY.Text);
        }

        private void SetProperties(SchemeProperties prop)
        {
            TextPageWidth.Text = prop.PageWidth.ToString();
            TextPageHeight.Text = prop.PageHeight.ToString();
            TextGridOriginX.Text = prop.GridOriginX.ToString();
            TextGridOriginY.Text = prop.GridOriginY.ToString();
            TextGridWidth.Text = prop.GridWidth.ToString();
            TextGridHeight.Text = prop.GridHeight.ToString();
            TextGridSize.Text = prop.GridSize.ToString();
            TextSnapX.Text = prop.SnapX.ToString();
            TextSnapY.Text = prop.SnapY.ToString();
            TextSnapOffsetX.Text = prop.SnapOffsetX.ToString();
            TextSnapOffsetY.Text = prop.SnapOffsetY.ToString();
        }

        private void DeselectAll()
        {
            var canvas = Editor.Context.CurrentCanvas;

            Editor.SelectNone();
            Editor.MouseEventRightDown(canvas);
        }

        private void MoveUp()
        {
            Editor.MoveUp(Editor.Context.CurrentCanvas);
        }

        private void MoveDown()
        {
            Editor.MoveDown(Editor.Context.CurrentCanvas);
        }

        private void MoveLeft()
        {
            Editor.MoveLeft(Editor.Context.CurrentCanvas);
        }

        private void MoveRight()
        {
            Editor.MoveRight(Editor.Context.CurrentCanvas);
        }

        private void Delete()
        {
            Editor.Delete();
        }

        #endregion

        #region Handle Key Events

        private void HandleKeyEvents(KeyEventArgs e)
        {
            var canvas = Editor.Context.CurrentCanvas;
            bool isControl = Keyboard.Modifiers == ModifierKeys.Control;
            bool canMove = e.OriginalSource is SchemeControl;
            var key = e.Key;

            if (isControl == true)
            {
                switch (key)
                {
                    case Key.E: break;
                    case Key.A: Editor.SelectAll(); break;
                    case Key.J:  break;
                    case Key.M: break;
                }
            }
            else
            {
                switch (key)
                {
                    case Key.Escape: DeselectAll(); break;
                    case Key.Delete: Delete(); break;
                    case Key.Up: if (canMove == true) { MoveUp(); e.Handled = true; } break;
                    case Key.Down: if (canMove == true) { MoveDown(); e.Handled = true; } break;
                    case Key.Left: if (canMove == true) { MoveLeft(); e.Handled = true; } break;
                    case Key.Right: if (canMove == true) { MoveRight(); e.Handled = true; } break;
                    case Key.I: break;
                    case Key.O: break;
                    case Key.F: InsertFEElement(canvas, GetInsertionPoint()); break;
                    case Key.S: Editor.ToggleWireStart(); break;
                    case Key.E: Editor.ToggleWireEnd(); break;
                    case Key.C: Connect(); break;
                    case Key.OemComma:  break;
                    case Key.OemPeriod:  break;
                    case Key.F5: break;
                    case Key.F6: break;
                    case Key.F7: break;
                    case Key.F8: break;
                    case Key.F9: TabOptions.IsSelected = true; break;
                }
            }
        }

        #endregion

        #region CheckBox Events

        private void EnableSnap_Click(object sender, RoutedEventArgs e)
        {
            Editor.Context.EnableSnap =
                EnableSnap.IsChecked == true ? true : false;
        }

        private void SnapOnRelease_Click(object sender, RoutedEventArgs e)
        {
            Editor.Context.SnapOnRelease =
                SnapOnRelease.IsChecked == true ? true : false;
        }

        private void EnablePage_Click(object sender, RoutedEventArgs e)
        {
            var Scheme = this.SchemeControl;
            var visibility = Scheme.Visibility;
            Scheme.Visibility = visibility == Visibility.Collapsed ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private void EnablePageTemplate_Click(object sender, RoutedEventArgs e)
        {
            var template = this.SchemeControl.SchemeTemplate;
            var visibility = template.Visibility;
            template.Visibility = visibility == Visibility.Collapsed ?
                Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Connect

        private void Connect()
        {
            var canvas = SchemeControl.SchemeCanvas;
            var point = GetInsertionPoint();
            if (point == null)
                return;

            var elements = this.HitTest(canvas, point, 6.0);
            var pin = elements.Where(x => x is PinThumb).FirstOrDefault();

            bool result = Editor.MouseEventPreviewLeftDown(canvas, point, pin as IThumb);
            if (result == false)
                Editor.MouseEventLeftDown(canvas, point);
        }

        public List<DependencyObject> HitTest(Visual visual, IPoint point, double radius)
        {
            var elements = new List<DependencyObject>();
            var elippse = new EllipseGeometry()
            {
                RadiusX = radius,
                RadiusY = radius,
                Center = new Point(point.X, point.Y),
            };

            var hitTestParams = new GeometryHitTestParameters(elippse);
            var resultCallback = new HitTestResultCallback(result => HitTestResultBehavior.Continue);

            var filterCallback = new HitTestFilterCallback(
                element =>
                {
                    elements.Add(element);
                    return HitTestFilterBehavior.Continue;
                });

            VisualTreeHelper.HitTest(visual, filterCallback, resultCallback, hitTestParams);

            return elements;
        }

        private PointEx GetInsertionPoint()
        {
            PointEx insertionPoint = null;

            var relativeTo = SchemeControl.SchemeCanvas;
            var point = Mouse.GetPosition(relativeTo);
            double x = point.X;
            double y = point.Y;
            double width = relativeTo.Width;
            double height = relativeTo.Height;

            if (x >= 0.0 && x <= width && y >= 0.0 && y <= height)
            {
                insertionPoint = new PointEx(x, y);
            }

            return insertionPoint;
        }

        #endregion

        #region Insert

        private void InsertFEElement(ICanvas canvas, PointEx point)
        {
            var element = Insert.FElement(canvas,
                point != null ? point : InsertPointGate, Editor.Context.SchemeCreator, Editor.Context.EnableSnap);

            Editor.SelectOneElement(element, true);
        }

        #endregion
    }
}
