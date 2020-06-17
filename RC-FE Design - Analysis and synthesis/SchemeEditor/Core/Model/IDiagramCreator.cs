﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
{
    public interface IDiagramCreator
    {
        void SetCanvas(ICanvas canvas);
        ICanvas GetCanvas();

        object CreateElement(string type, object[] data, double x, double y, bool snap);
        object CreateDiagram(DiagramProperties properties);
        object CreateGrid(double originX, double originY, double width, double height, double size);

        void UpdateConnections(IDictionary<string, Child> dict);
        void UpdateCounter(IdCounter original, IdCounter counter);
        void AppendIds(IEnumerable<object> elements);
        void InsertElements(IEnumerable<object> elements, bool select, double offsetX, double offsetY);
    } 
}
