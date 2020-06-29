﻿using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public interface ISchemeCreator
    {
        void SetCanvas(ICanvas canvas);
        ICanvas GetCanvas();

        object CreateElement(string type, object[] data, double x, double y, bool snap);
        object CreateScheme(SchemeProperties properties);

        void UpdateConnections(IDictionary<string, Child> dict);
        void UpdateCounter(IdCounter original, IdCounter counter);
        void AppendIds(IEnumerable<object> elements);
        void InsertElements(IEnumerable<object> elements, bool select, double offsetX, double offsetY);
    } 
}
