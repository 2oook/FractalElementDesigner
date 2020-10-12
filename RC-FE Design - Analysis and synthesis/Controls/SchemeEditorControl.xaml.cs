using FractalElementDesigner.SchemeEditing.Core;
using FractalElementDesigner.SchemeEditing.Controls;
using FractalElementDesigner.SchemeEditing.Editor;
using FractalElementDesigner.SchemeEditing;
using FractalElementDesigner.SchemeEditing.Core.Model;
using FractalElementDesigner.SchemeEditing.Views;
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

namespace FractalElementDesigner.Controls
{
    /// <summary>
    /// Interaction logic for SchemeEditorControl.xaml
    /// </summary>
    public partial class SchemeEditorControl : UserControl
    {

        public SchemeEditorControl()
        {
            InitializeComponent();

            InitializeEditor();
            InitializeSchemeControl();
            InitializeWindowEvents();
        }

        #region Fields

        public SchemeEditing.Editor.SchemeEditor Editor { get; private set; }

        private PointEx InsertDefaultPoint { get; set; } = new PointEx(325.0, 30.0);

        #endregion

        #region Initialize

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

        public void InitializeEditor()
        {
            Editor = new SchemeEditor();
            Editor.Context = new Context();
            Editor.Context.CurrentCanvas = this.SchemeControl.SchemeCanvas;

            var counter = new IdCounter();
            counter.Set(3);
            this.SchemeControl.SchemeCanvas.SetCounter(counter);

            var prop = SchemeProperties.Default;
            this.SchemeControl.SchemeCanvas.SetProperties(prop);

            Editor.Context.IsControlPressed = () => Keyboard.Modifiers == ModifierKeys.Control;

            // Scheme creator
            Editor.Context.SchemeCreator = GetSchemeCreator();
        }

        public SchemeEditor InitializeNewEditor(ICanvas canvas)
        {
            var editor = new SchemeEditor();
            editor.Context = new Context();
            editor.Context.CurrentCanvas = canvas;

            var counter = new IdCounter();
            counter.Set(3);
            canvas.SetCounter(counter);

            var prop = SchemeProperties.Default;
            canvas.SetProperties(prop);

            editor.Context.IsControlPressed = () => Keyboard.Modifiers == ModifierKeys.Control;

            var creator = new WpfSchemeCreator();

            creator.SetThumbEvents = (thumb) => SetThumbEvents(thumb);
            creator.SetPosition = (element, left, top, snap) => editor.SetPosition(element, left, top, snap);

            creator.GetCounter = () => editor.Context.CurrentCanvas.GetCounter();
            creator.SetCanvas(canvas);

            // Scheme creator
            editor.Context.SchemeCreator = creator;

            return editor;
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
            //thumb.DragDelta += (sender, e) =>
            //{
            //    var canvas = Editor.Context.CurrentCanvas;
            //    var element = sender as IThumb;

            //    double dX = e.HorizontalChange;
            //    double dY = e.VerticalChange;

            //    Editor.Drag(canvas, element, dX, dY);
            //};

            //thumb.DragStarted += (sender, e) =>
            //{
            //    var canvas = Editor.Context.CurrentCanvas;
            //    var element = sender as IThumb;

            //    Editor.DragStart(canvas, element);
            //};

            //thumb.DragCompleted += (sender, e) =>
            //{
            //    var canvas = Editor.Context.CurrentCanvas;
            //    var element = sender as IThumb;

            //    Editor.DragEnd(canvas, element);
            //};
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
                    case Key.A: Editor.SelectAll(); break;
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
                    case Key.F: InsertFEElement(canvas, GetInsertionPoint()); break;
                    case Key.S: Editor.ToggleWireStart(); break;
                    case Key.E: Editor.ToggleWireEnd(); break;
                    case Key.C: Connect(); break;
                }
            }
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
            if (result == false) Editor.MouseEventLeftDown(canvas, point);
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
                point != null ? point : InsertDefaultPoint, Editor.Context.SchemeCreator, Editor.Context.EnableSnap);

            Editor.SelectOneElement(element, true);
        }

        #endregion
    }
}
