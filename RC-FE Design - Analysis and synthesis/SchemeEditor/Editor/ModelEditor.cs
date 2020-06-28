using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model;
using RC_FE_Design___Analysis_and_synthesis.SchemeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Editor
{
    public static class ModelEditor
    {
        private static string DefaultUid = Constants.TagHeaderScheme + Constants.TagNameSeparator + (-1).ToString();

        #region Clear

        public static void Clear(ICanvas canvas)
        {
            canvas.Clear();
        }

        #endregion

        #region Elements

        public static IEnumerable<IElement> GetSelected(ICanvas canvas)
        {
             return canvas
                 .GetElements()
                 .OfType<IElement>()
                 .Where(x => x.GetSelected() == true)
                 .ToList();
        }

        public static IEnumerable<IElement> GetSelectedThumbs(ICanvas canvas)
        {
            return canvas
                .GetElements()
                .OfType<IThumb>()
                .Where(x => x.GetSelected() == true)
                .Cast<IElement>()
                .ToList();
        }

        public static IEnumerable<IElement> GetSelectedWires(ICanvas canvas)
        {
            return canvas
                .GetElements()
                .OfType<ILine>()
                .Where(x => x.GetSelected() == true)
                .Cast<IElement>()
                .ToList();
        }

        public static IEnumerable<IElement> GetAll(ICanvas canvas)
        {
            return canvas
                .GetElements()
                .OfType<IElement>()
                .ToList();
        }

        public static IEnumerable<IElement> GetThumbs(ICanvas canvas)
        {
            return canvas
                .GetElements()
                .OfType<IThumb>()
                .Cast<IElement>()
                .ToList();
        }

        public static IEnumerable<IElement> GetWires(ICanvas canvas)
        {
            return canvas
                .GetElements()
                .OfType<ILine>()
                .Cast<IElement>()
                .ToList();
        }

        #endregion

        #region Move

        private static void MoveElement(IElement element, double dX, double dY)
        {
            if (dX != 0.0)
                element.SetX(element.GetX() - dX);

            if (dY != 0.0)
                element.SetY(element.GetY() - dY);
        }

        public static void MoveLine(ILine line, double dX, double dY)
        {
            MoveLineStart(line, dX, dY);
            MoveLineEnd(line, dX, dY);
        }

        public static void MoveLineEnd(ILine line, double dX, double dY)
        {
            double left = line.GetX2();
            double top = line.GetY2();
            double x = dX != 0.0 ? left - dX : left;
            double y = dY != 0.0 ? top - dY : top;

            line.SetX2(x);
            line.SetY2(y);
        }

        public static void MoveLineStart(ILine line, double dX, double dY)
        {
            var margin = line.GetMargin();
            double left = margin.Left;
            double top = margin.Top;
            double x = dX != 0.0 ? left - dX : left;
            double y = dY != 0.0 ? top - dY : top;

            line.SetX2(line.GetX2() + (left - x));
            line.SetY2(line.GetY2() + (top - y));
            line.SetMargin(new MarginEx(0, x, 0, y));
        }

        #endregion

        #region Insert

        public static void Insert(ICanvas canvas, 
            IEnumerable<IElement> elements, 
            bool select, 
            double offsetX, 
            double offsetY)
        {
            var thumbs = elements.Where(x => x is IThumb);
            int count = thumbs.Count();
            double minX = count == 0 ? 0.0 : thumbs.Min(x => x.GetX());
            double minY = count == 0 ? 0.0 : thumbs.Min(x => x.GetY());
            double dX = offsetX != 0.0 ? minX - offsetX : 0.0;
            double dY = offsetY != 0.0 ? minY - offsetY : 0.0;

            foreach (var element in elements)
            {
                canvas.Add(element);

                if (element is IThumb)
                    MoveElement(element, dX, dY);
                else if (element is ILine && (dX != 0.0 || dY != 0.0))
                    MoveLine(element as ILine, dX, dY);

                if (select == true)
                    element.SetSelected(true);
            }
        }

        #endregion

        #region Selection

        public static void SelectionToggleWire(IElement element)
        {
            string uid = element.GetUid();

            if (element is ILine && uid != null &&
                StringHelper.StartsWith(uid, Constants.TagElementWire))
            {
                var line = element as ILine;
                line.SetSelected(line.GetSelected() ? false : true);
            }
        }

        public static void SetThumbsSelection(ICanvas canvas, bool isSelected)
        {
            foreach (var thumb in canvas.GetElements().OfType<IThumb>())
                thumb.SetSelected(isSelected);
        }

        public static void SetLinesSelection(ICanvas canvas, bool isSelected)
        {
            foreach (var line in canvas.GetElements().OfType<ILine>())
                line.SetSelected(isSelected);
        }

        public static void SelectAll(ICanvas canvas)
        {
            SetThumbsSelection(canvas, true);
            SetLinesSelection(canvas, true);
        }

        public static void SelectNone(ICanvas canvas)
        {
            SetThumbsSelection(canvas, false);
            SetLinesSelection(canvas, false);
        }

        #endregion

        #region Select Connected

        public static void SelectConnected(ICanvas canvas)
        {
            var elements = GetSelectedThumbs(canvas);

            if (elements != null)
            {
                var element = elements.FirstOrDefault();
                if (element != null)
                {
                    SelectNone(canvas);

                    var visited = new HashSet<string>();
                    SelectConnected(element, visited);
                    visited = null;
                }
            }
        }

        public static void SelectConnected(IElement element, HashSet<string> visited)
        {
            if (element == null)
                return;

            var tag = element.GetTag();
            if (tag == null)
                return;

            visited.Add(element.GetUid());
            element.SetSelected(true);

            foreach (var wire in (tag as Connection).Wires)
                SelectConnected(wire, element, visited);
        }

        public static void SelectConnected(Wire wire, IElement root, HashSet<string> visited)
        {
            var line = wire.Line as ILine;
            var tag = line.GetTag() as Wire;

            line.SetSelected(true);
            if (tag == null)
                return;

            if (CanSelectStart(root, visited, tag.Start))
                SelectConnected(tag.Start, visited);

            if (CanSelectEnd(root, visited, tag.End))
                SelectConnected(tag.End, visited);
        }

        private static bool CanSelectStart(IElement root, HashSet<string> visited, IElement startRoot)
        {
            return startRoot != null &&
                StringHelper.Compare(startRoot.GetUid(), root.GetUid()) == false &&
                visited.Contains(startRoot.GetUid()) == false;
        }

        private static bool CanSelectEnd(IElement root, HashSet<string> visited, IElement endRoot)
        {
            return endRoot != null &&
                StringHelper.Compare(endRoot.GetUid(), root.GetUid()) == false &&
                visited.Contains(endRoot.GetUid()) == false;
        }

        #endregion

        #region IDs

        public static void IdsAppend(IEnumerable<object> elements, IdCounter counter)
        {
            foreach (var element in elements.Cast<IElement>())
                element.SetUid(GetUid(counter, element));
        }

        private static string GetUid(IdCounter counter, IElement element)
        {
            return string.Concat(element.GetUid().Split(Constants.TagNameSeparator)[0], 
                Constants.TagNameSeparator, 
                counter.Next().ToString());
        }

        public static void IdsUpdateCounter(IdCounter original, IdCounter counter)
        {
            original.Set(Math.Max(original.Count, counter.Count));
        }

        #endregion

        #region Connections

        public static void ConnectionsUpdate(IDictionary<string, Child> dict)
        {
            foreach (var item in dict)
            {
                var element = item.Value.Element as IElement;
                if (element == null)
                    continue;

                if (element.GetTag() == null)
                    element.SetTag(new Connection(element, new List<Wire>()));

                var pins = item.Value.Pins;
                if (pins.Count > 0)
                    UpdateWires(dict, element, pins);
            }
        }

        private static void UpdateWires(IDictionary<string, Child> dict, IElement element, List<Pin> pins)
        {
            var connection = element.GetTag() as Connection;
            var wires = connection.Wires;

            foreach (var pin in pins)
            {
                string name = pin.Name;
                string type = pin.Type;

                if (StringHelper.Compare(type, Constants.WireStartType))
                {
                    Child child = null;
                    if (dict.TryGetValue(name, out child) == true)
                    {
                        var line = child.Element;
                        if (line == null)
                            continue;

                        UpdateStartTag(element, wires, line);
                    }
                    else
                        System.Diagnostics.Debug.Print("Failed to map wire Start: {0}", name);
                }
                else if (StringHelper.Compare(type, Constants.WireEndType))
                {
                    Child child = null;
                    if (dict.TryGetValue(name, out child) == true)
                    {
                        var line = child.Element;
                        if (line == null)
                            continue;

                        UpdateEndTag(element, wires, line);
                    }
                    else
                        System.Diagnostics.Debug.Print("Failed to map wire End: {0}", name);
                }
            }
        }

        private static void UpdateStartTag(IElement element, List<Wire> wires, object line)
        {
            wires.Add(new Wire(line, element, null));

            var lineEx = line as ILine;
            if (lineEx.GetTag() != null)
            {
                var root = lineEx.GetTag() as IElement;
                if (root != null)
                    lineEx.SetTag(new Wire(lineEx, element, root));
            }
            else
            {
                lineEx.SetTag(element);
            }
        }

        private static void UpdateEndTag(IElement element, List<Wire> wires, object line)
        {
            wires.Add(new Wire(line, null, element));

            var lineEx = line as ILine;
            if (lineEx.GetTag() != null)
            {
                var root = lineEx.GetTag() as IElement;
                if (root != null)
                    lineEx.SetTag(new Wire(lineEx, root, element));
            }
            else
            {
                lineEx.SetTag(element);
            }
        }

        public static void GetPinPosition(IElement root, IThumb pin, out double x, out double y)
        {
            x = root.GetX() + pin.GetX();
            y = root.GetY() + pin.GetY();
        }

        #endregion

        #region Delete

        public static void DeleteElement(ICanvas canvas, IPoint point)
        {
            var element = canvas.HitTest(point, 6.0).FirstOrDefault() as IElement;
            if (element == null)
                return;

            DeleteElement(canvas, element);
        }

        public static void DeleteElement(ICanvas canvas, IElement element)
        {
            string uid = element.GetUid();

            if (element is ILine && uid != null && StringHelper.StartsWith(uid, Constants.TagElementWire))
                DeleteWire(canvas, element as ILine);
            else
                canvas.Remove(element);
        }

        public static void DeleteWire(ICanvas canvas, ILine line)
        {
            canvas.Remove(line);

            RemoveWireConnections(canvas, line);
            DeleteEmptyPins(canvas);
        }

        public static void DeleteEmptyPins(ICanvas canvas)
        {
            foreach (var pin in FindEmptyPins(canvas))
                canvas.Remove(pin);
        }

        public static List<IElement> FindEmptyPins(ICanvas canvas)
        {
            var empty = new List<IElement>();

            foreach (var element in canvas.GetElements())
            {
                if (IsElementPin(element.GetUid()))
                {
                    var tag = element.GetTag();
                    if (tag == null)
                        empty.Add(element);
                    else if ((tag as Connection).Wires.Count <= 0)
                        empty.Add(element);
                }
            }

            return empty;
        }

        public static bool IsElementPin(string uid)
        {
            return uid != null &&
                   StringHelper.StartsWith(uid, Constants.TagElementPin);
        }

        public static List<Connection> RemoveWireConnections(ICanvas canvas, ILine line)
        {
            var connections = new List<Connection>();

            foreach (var element in canvas.GetElements())
            {
                if (element.GetTag() != null && !(element is ILine))
                    RemoveWireConnections(line, connections, element);
            }

            return connections;
        }

        public static void RemoveWireConnections(ILine line, List<Connection> connections, IElement element)
        {
            var wires = (element.GetTag() as Connection).Wires;
            var maps = CreateMapWire(line, wires);

            if (maps.Count > 0)
                connections.Add(new Connection(element, maps));

            foreach (var map in maps)
                wires.Remove(map);
        }

        private static List<Wire> CreateMapWire(ILine line, List<Wire> wires)
        {
            var map = new List<Wire>();

            foreach (var wire in wires)
            {
                if (StringHelper.Compare((wire.Line as ILine).GetUid(), line.GetUid()))
                    map.Add(wire);
            }

            return map;
        }

        #endregion
    }
}
