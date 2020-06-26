using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Controls;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Editor;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor;
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
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Views;

namespace RC_FE_Design___Analysis_and_synthesis.Pages
{
    /// <summary>
    /// Interaction logic for SchemeEditorPage.xaml
    /// </summary>
    public partial class SchemeEditorPage : Page
    {
        #region Fields

        private string ResourcesUri = "ElementsDictionary.xaml";

        private SchemeEditor.Editor.SchemeEditor Editor { get; set; }

        private PointEx InsertPointInput = new PointEx(30, 30.0);
        private PointEx InsertPointOutput = new PointEx(930.0, 30.0);
        private PointEx InsertPointGate = new PointEx(325.0, 30.0);

        private double PageWidth = 1260.0;
        private double PageHeight = 891.0;

        private double GuideSpeedUpLevel1 = 1.0;
        private double GuideSpeedUpLevel2 = 2.0;

        private string WindowDefaultTitle = "Canvas Diagram Editor";
        private string WindowTitleDirtyString = "*";
        private string WindowTitleSeparator = " - ";

        private string SolutionNewFileName = "Solution0";
        private bool SolutionIsDirty = false;
        private string SolutionFileName = null;

        private string TagsNewFileName = "Tags0";

        #endregion

        #region Constructor

        public SchemeEditorPage()
        {
            InitializeComponent();

            InitializeEditor();
            InitializeDiagramControl();
            InitializeWindowEvents();
            InitializeEditMenuEvents();
        }

        #endregion

        #region Window Title

        private void UpdateWindowTitle()
        {
            if (SolutionFileName == null && SolutionIsDirty == false)
            {
                string title = string.Concat(SolutionNewFileName,
                    WindowTitleSeparator,
                    WindowDefaultTitle);

                this.Title = title;
            }
            else if (SolutionFileName == null && SolutionIsDirty == true)
            {
                string title = string.Concat(SolutionNewFileName,
                    WindowTitleDirtyString,
                    WindowTitleSeparator,
                    WindowDefaultTitle);

                this.Title = title;
            }
            else if (SolutionFileName != null && SolutionIsDirty == false)
            {
                string title = string.Concat(System.IO.Path.GetFileName(SolutionFileName),
                    WindowTitleSeparator,
                    WindowDefaultTitle);

                this.Title = title;
            }
            else if (SolutionFileName != null && SolutionIsDirty == true)
            {
                string title = string.Concat(System.IO.Path.GetFileName(SolutionFileName),
                    WindowTitleDirtyString,
                    WindowTitleSeparator,
                    WindowDefaultTitle);

                this.Title = title;
            }
            else
            {
                this.Title = WindowDefaultTitle;
            }
        }

        private void UpdateSolutionState(bool isDirty, string fileName)
        {
            SolutionIsDirty = isDirty;
            SolutionFileName = fileName;

            UpdateWindowTitle();
        }

        #endregion

        #region Initialize

        private void InitializeEditMenuEvents()
        {
            EditDelete.Click += (sender, e) => Delete();
            EditSelectAll.Click += (sender, e) => Editor.SelectAll();
            EditDeselectAll.Click += (sender, e) => DeselectAll();
            EditSelectPrevious.Click += (sender, e) => Editor.SelectPrevious(!(Keyboard.Modifiers == ModifierKeys.Control));
            EditSelectNext.Click += (sender, e) => Editor.SelectNext(!(Keyboard.Modifiers == ModifierKeys.Control));
            EditSelectConnected.Click += (sender, e) => Editor.SelectConnected();
            EditClear.Click += (sender, e) => Editor.ClearCanvas();
            EditResetThumbTags.Click += (sender, e) => Editor.ResetThumbTags();
            EditConnect.Click += (sender, e) => Connect();
        }

        private void InitializeDiagramControl()
        {
            this.DiagramControl.Editor = this.Editor;
        }

        private void InitializeWindowEvents()
        {
            this.Loaded += (sender, e) =>
            {
                this.DiagramControl.Focus();
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
            Editor.Context.CurrentCanvas = this.DiagramControl.DiagramCanvas;

            var counter = new IdCounter();
            counter.Set(3);
            this.DiagramControl.DiagramCanvas.SetCounter(counter);

            var prop = SchemeProperties.Default;
            this.DiagramControl.DiagramCanvas.SetProperties(prop);
            SetProperties(prop);

            Editor.Context.IsControlPressed = () => Keyboard.Modifiers == ModifierKeys.Control;
            Editor.Context.UpdateProperties = () => UpdateProperties(Editor.Context.CurrentCanvas.GetProperties());
            Editor.Context.SetProperties = (p) => SetProperties(p);

            // diagram creator
            Editor.Context.DiagramCreator = GetDiagramCreator();

            // set checkbox states
            EnableInsertLast.IsChecked = Editor.Context.EnableInsertLast;
            EnableSnap.IsChecked = Editor.Context.EnableSnap;
            SnapOnRelease.IsChecked = Editor.Context.SnapOnRelease;
        }

        private ISchemeCreator GetDiagramCreator()
        {
            var creator = new WpfSchemeCreator();

            creator.SetThumbEvents = (thumb) => SetThumbEvents(thumb);
            creator.SetPosition = (element, left, top, snap) => Editor.SetPosition(element, left, top, snap);

            creator.GetCounter = () => Editor.Context.CurrentCanvas.GetCounter();
            creator.SetCanvas(this.DiagramControl.DiagramCanvas);


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

        private double CalculateMoveSpeedUp(int delta)
        {
            return (delta > -200.0 && delta < -50.0) ?
                GuideSpeedUpLevel1 : (delta > -50.0) ?
                GuideSpeedUpLevel2 : 1.0;
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
                    case Key.R: Editor.ResetThumbTags(); break;
                    case Key.E: break;
                    case Key.A: Editor.SelectAll(); break;
                    case Key.OemOpenBrackets: Editor.SelectPrevious(false); break;
                    case Key.OemCloseBrackets: Editor.SelectNext(false); break;
                    case Key.J:  break;
                    case Key.M: break;
                }
            }
            else
            {
                switch (key)
                {
                    case Key.OemOpenBrackets: Editor.SelectPrevious(true); break;
                    case Key.OemCloseBrackets: Editor.SelectNext(true); break;
                    case Key.OemPipe: Editor.SelectConnected(); break;
                    case Key.Escape: DeselectAll(); break;
                    case Key.Delete: Delete(); break;
                    case Key.Up: if (canMove == true) { MoveUp(); e.Handled = true; } break;
                    case Key.Down: if (canMove == true) { MoveDown(); e.Handled = true; } break;
                    case Key.Left: if (canMove == true) { MoveLeft(); e.Handled = true; } break;
                    case Key.Right: if (canMove == true) { MoveRight(); e.Handled = true; } break;
                    case Key.I: break;
                    case Key.O: break;
                    case Key.R: break;
                    case Key.A: InsertAndGate(canvas, GetInsertionPoint()); break;
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

        private void EnableInsertLast_Click(object sender, RoutedEventArgs e)
        {
            Editor.Context.EnableInsertLast =
                EnableInsertLast.IsChecked == true ? true : false;
        }

        private void EnablePage_Click(object sender, RoutedEventArgs e)
        {
            var diagram = this.DiagramControl;
            var visibility = diagram.Visibility;
            diagram.Visibility = visibility == Visibility.Collapsed ?
                Visibility.Visible : Visibility.Collapsed;
        }


        private void EnablePageTemplate_Click(object sender, RoutedEventArgs e)
        {
            var template = this.DiagramControl.DiagramTemplate;
            var visibility = template.Visibility;
            template.Visibility = visibility == Visibility.Collapsed ?
                Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Button Events

        private void DefaultZoom()
        {
            DiagramControl.ResetZoom();
            DiagramControl.ResetPan();
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            DefaultZoom();
        }

        #endregion

        #region Connect

        private void Connect()
        {
            var canvas = DiagramControl.DiagramCanvas;
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

            var relativeTo = DiagramControl.DiagramCanvas;
            var point = Mouse.GetPosition(relativeTo);
            double x = point.X;
            double y = point.Y;
            double width = relativeTo.Width;
            double height = relativeTo.Height;

            if (x >= 0.0 && x <= width &&
                y >= 0.0 && y <= height)
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
                point != null ? point : InsertPointGate, Editor.Context.DiagramCreator, Editor.Context.EnableSnap);

            Editor.SelectOneElement(element, true);
        }

        private void InsertAndGate(ICanvas canvas, PointEx point)
        {
            var element = Insert.AndGate(canvas,
                point != null ? point : InsertPointGate, Editor.Context.DiagramCreator, Editor.Context.EnableSnap);

            Editor.SelectOneElement(element, true);
        }

        #endregion
    }
}
