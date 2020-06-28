using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Editor
{
    public class SchemeEditor
    {
        #region Properties

        public Context Context { get; set; }

        #endregion

        #region Model

        public void ClearCanvas()
        {
            ModelEditor.Clear(Context.CurrentCanvas);
        }

        #endregion

        #region Wire Connection

        private void Connect(ICanvas canvas, IThumb pin, ISchemeCreator creator)
        {
            if (pin == null)
                return;

            Context.CurrentRoot = pin.GetParent() as IThumb;

            double x;
            double y;
            ModelEditor.GetPinPosition(Context.CurrentRoot, pin, out x, out y);

            Context.CurrentLine = WireEditor.Connect(canvas, 
                Context.CurrentRoot, Context.CurrentLine, 
                x, y,
                creator);

            if (Context.CurrentLine == null)
                Context.CurrentRoot = null;
        }

        #endregion

        #region Add

        public IElement Add(ICanvas canvas, string type, IPoint point)
        {
            var context = Context.SchemeCreator;
            var snap = Context.EnableSnap;
            switch (type)
            {
                case Constants.TagElementFElement: return Insert.FElement(canvas, point, context, snap);
                case Constants.TagElementAndGate: return Insert.AndGate(canvas, point, context, snap);
                
                case Constants.TagElementPin: return Insert.Pin(canvas, point, context, snap);
                default: return null;
            }
        }

        #endregion

        #region Move

        public void SetPosition(IElement element, double left, double top, bool snap)
        {
            element.SetX(SnapOffsetX(left, snap));
            element.SetY(SnapOffsetY(top, snap));
        }

        private void MoveRoot(IElement element, double dX, double dY, bool snap)
        {
            SetPosition(element, element.GetX() + dX, element.GetY() + dY, snap);
            MoveLines(element, dX, dY, snap);
        }

        private void MoveLines(IElement element, double dX, double dY, bool snap)
        {
            if (element == null || element.GetTag() == null)
                return;

            var connection = element.GetTag() as Connection;
            foreach (var wire in connection.Wires)
                MoveLine(dX, dY, snap, wire);
        }

        private void MoveLine(double dX, double dY, bool snap, Wire wire)
        {
            var line = wire.Line as ILine;
            var start = wire.Start;
            var end = wire.End;

            if (start != null)
            {
                var margin = line.GetMargin();
                double left = margin.Left;
                double top = margin.Top;
                //line.X1 = SnapOffsetX(line.X1 + dX, snap);
                //line.Y1 = SnapOffsetY(line.Y1 + dY, snap);
                double x = SnapOffsetX(left + dX, snap);
                double y = SnapOffsetY(top + dY, snap);

                if (left != x || top != y)
                {
                    line.SetX2(line.GetX2() + (left - x));
                    line.SetY2(line.GetY2() + (top - y));
                    line.SetMargin(new MarginEx(0, x, 0, y));
                }
            }

            if (end != null)
            {
                double left = line.GetX2();
                double top = line.GetY2();
                double x = SnapX(left + dX, snap);
                double y = SnapY(top + dY, snap);
                line.SetX2(x);
                line.SetY2(y);
            }
        }

        public void MoveSelectedElements(ICanvas canvas, double dX, double dY, bool snap)
        {
            var thumbs = canvas.GetElements().OfType<IThumb>().Where(x => x.GetSelected());
            foreach (var thumb in thumbs)
                MoveRoot(thumb, dX, dY, snap);
        }

        public void MoveLeft(ICanvas canvas)
        {
            MoveSelectedElements(canvas, Context.EnableSnap ? -canvas.GetProperties().GridSize : -1.0, 0.0, false);
        }

        public void MoveRight(ICanvas canvas)
        {
            MoveSelectedElements(canvas, Context.EnableSnap ? canvas.GetProperties().GridSize : 1.0, 0.0, false);
        }

        public void MoveUp(ICanvas canvas)
        {
            MoveSelectedElements(canvas, 0.0, Context.EnableSnap ? -canvas.GetProperties().GridSize : -1.0, false);
        }

        public void MoveDown(ICanvas canvas)
        {
            MoveSelectedElements(canvas, 0.0, Context.EnableSnap ? canvas.GetProperties().GridSize : 1.0, false);
        }

        #endregion

        #region Drag

        private bool IsSnapOnDragEnabled()
        {
            return (Context.SnapOnRelease == true &&
                Context.EnableSnap == true) ? false : Context.EnableSnap;
        }

        public void Drag(ICanvas canvas, IThumb element, double dX, double dY)
        {
            bool snap = IsSnapOnDragEnabled();
            if (Context.MoveAllSelected == true)
                MoveSelectedElements(canvas, dX, dY, snap);
            else
                MoveRoot(element, dX, dY, snap);
        }

        public void DragStart(ICanvas canvas, IThumb element)
        {
            Context.MoveAllSelected = element.GetSelected();
            if (Context.MoveAllSelected == false)
                element.SetSelected(true);
        }

        public void DragEnd(ICanvas canvas, IThumb element)
        {
            if (Context.SnapOnRelease == true && Context.EnableSnap == true)
            {
                if (Context.MoveAllSelected == true)
                {
                    MoveSelectedElements(canvas, 0.0, 0.0, Context.EnableSnap);
                }
                else
                {
                    element.SetSelected(false);
                    MoveRoot(element, 0.0, 0.0, Context.EnableSnap);
                }
            }
            else
            {
                if (Context.MoveAllSelected != true)
                    element.SetSelected(false);
            }

            Context.MoveAllSelected = false;
        }

        #endregion

        #region Wire

        public void ToggleWireStart()
        {
            var wires = ModelEditor.GetSelectedWires(Context.CurrentCanvas);
            if (wires.Count() <= 0)
                return;

            foreach (var wire in wires.Cast<ILine>())
                wire.SetStartVisible(wire.GetStartVisible() == true ? false : true);
        }

        public void ToggleWireEnd()
        {
            var wires = ModelEditor.GetSelectedWires(Context.CurrentCanvas);
            if (wires.Count() <= 0)
                return;

            foreach (var wire in wires.Cast<ILine>())
                wire.SetEndVisible(wire.GetEndVisible() == true ? false : true);
        }

        #endregion

        #region Edit

        public void Delete()
        {
            Delete(Context.CurrentCanvas, ModelEditor.GetSelected(Context.CurrentCanvas));
        }

        public void Delete(ICanvas canvas, IPoint point)
        {
            ModelEditor.DeleteElement(canvas, point);

            Context.SkipLeftClick = false;
        }

        public void Delete(ICanvas canvas, IEnumerable<IElement> elements)
        {
            foreach (var element in elements)
                ModelEditor.DeleteElement(canvas, element);
        }

        #endregion

        #region Selection

        public void SelectAll()
        {
            ModelEditor.SelectAll(Context.CurrentCanvas);
        }

        public void SelectNone()
        {
            ModelEditor.SelectNone(Context.CurrentCanvas);
        }

        public void SelectOneElement(IElement element, bool deselect)
        {
            if (element == null)
                return;

            if (deselect == true)
            {
                SelectNone();
                element.SetSelected(true);
            }
            else
                element.SetSelected(!element.GetSelected());
        }

        #endregion

        #region Mouse Conditions

        public bool IsSameUid(string uid1, string uid2)
        {
            return StringHelper.Compare(uid2, uid1) == false;
        }

        public bool IsWire(string elementUid)
        {
            return StringHelper.StartsWith(elementUid, Constants.TagElementWire) == true;
        }

        public bool CanConnect()
        {
            return Context.CurrentRoot != null && Context.CurrentLine != null;
        }

        public bool CanSplitWire(IElement element)
        {
            if (element == null)
                return false;

            var elementUid = element.GetUid();
            var lineUid = Context.CurrentLine.GetUid();

            return element != null &&
                CanConnect() &&
                IsSameUid(elementUid, lineUid) &&
                IsWire(elementUid);
        }

        public bool CanToggleLine()
        {
            return Context.CurrentRoot == null &&
                Context.CurrentLine == null &&
                (Context.IsControlPressed != null && Context.IsControlPressed());
        }

        public bool CanConnectToPin(IThumb pin)
        {
            return pin != null &&
            (
                !StringHelper.Compare(pin.GetUid(), Constants.PinStandalone) ||
                (Context.IsControlPressed != null && Context.IsControlPressed())
            );
        }

        public bool CanMoveCurrentLine()
        {
            return Context.CurrentRoot != null &&
                Context.CurrentLine != null;
        }

        #endregion

        #region Mouse Helpers

        private void MouseCreateCanvasConnection(ICanvas canvas, IPoint point)
        {
            var root = Insert.Pin(canvas, point, Context.SchemeCreator, Context.EnableSnap);

            Context.CurrentRoot = root;

            double x = Context.CurrentRoot.GetX();
            double y = Context.CurrentRoot.GetY();

            Context.CurrentLine = WireEditor.Connect(canvas, Context.CurrentRoot, Context.CurrentLine, x, y, Context.SchemeCreator);
            if (Context.CurrentLine == null)
                Context.CurrentRoot = null;

            Context.CurrentRoot = root;
            Context.CurrentLine = WireEditor.Connect(canvas, Context.CurrentRoot, Context.CurrentLine, x, y, Context.SchemeCreator);
        }

        private IElement MouseGetElementAtPoint(ICanvas canvas, IPoint point)
        {
            var element = canvas.HitTest(point, 6.0)
                .Where(x => StringHelper.Compare(Context.CurrentLine.GetUid(), x.GetUid()) == false)
                .FirstOrDefault() as IElement;

            return element;
        }

        private void MouseRemoveCurrentLine(ICanvas canvas)
        {
            var connection = Context.CurrentRoot.GetTag() as Connection;
            var wires = connection.Wires;
            var last = wires.LastOrDefault();

            wires.Remove(last);
            canvas.Remove(Context.CurrentLine);
        }

        private void MouseToggleLineSelection(ICanvas canvas, IPoint point)
        {
            var element = canvas.HitTest(point, 6.0).FirstOrDefault() as IElement;

            if (element != null)
                ModelEditor.SelectionToggleWire(element);
            else
                ModelEditor.SetLinesSelection(canvas, false);
        }

        #endregion

        #region Mouse Event

        public void MouseEventLeftDown(ICanvas canvas, IPoint point)
        {
            if (CanConnect())
            {
                 MouseCreateCanvasConnection(canvas, point);
            }
        }

        public bool MouseEventPreviewLeftDown(ICanvas canvas, IPoint point, IThumb pin)
        {
            if (CanConnectToPin(pin))
            {
                Connect(canvas, pin, Context.SchemeCreator);

                return true;
            }
            else if (Context.CurrentLine != null)
            {
                var element = MouseGetElementAtPoint(canvas, point);
                if (CanSplitWire(element))
                {

                    bool result = WireEditor.Split(canvas, element as ILine, Context.CurrentLine, point, Context.SchemeCreator, Context.EnableSnap);

                    Context.CurrentRoot = null;
                    Context.CurrentLine = null;

                    return result;
                }
            }

            if (CanToggleLine())
                MouseToggleLineSelection(canvas, point);

            return false;
        }

        public void MouseEventMove(ICanvas canvas, IPoint point)
        {
            if (CanMoveCurrentLine())
            {
                var margin = Context.CurrentLine.GetMargin();
                double x = point.X - margin.Left;
                double y = point.Y - margin.Top;

                if (Context.CurrentLine.GetX2() != x)
                    Context.CurrentLine.SetX2(x);

                if (Context.CurrentLine.GetY2() != y)
                    Context.CurrentLine.SetY2(y);
            }
        }

        public bool MouseEventRightDown(ICanvas canvas)
        {
            if (Context.CurrentRoot != null && Context.CurrentLine != null)
            {
                Context.CurrentLine = null;
                Context.CurrentRoot = null;

                return true;
            }

            return false;
        }

        #endregion

        #region Snap

        public double SnapOffsetX(double original, bool snap)
        {
            var prop = Context.CurrentCanvas.GetProperties();
            return snap == true ? SnapHelper.Snap(original, prop.SnapX, prop.SnapOffsetX) : original;
        }

        public double SnapOffsetY(double original, bool snap)
        {
            var prop = Context.CurrentCanvas.GetProperties();
            return snap == true ? SnapHelper.Snap(original, prop.SnapY, prop.SnapOffsetY) : original;
        }

        public double SnapX(double original, bool snap)
        {
            var prop = Context.CurrentCanvas.GetProperties();
            return snap == true ? SnapHelper.Snap(original, prop.SnapX) : original;
        }

        public double SnapY(double original, bool snap)
        {
            var prop = Context.CurrentCanvas.GetProperties();
            return snap == true ? SnapHelper.Snap(original, prop.SnapY) : original;
        }

        #endregion
    }
}
