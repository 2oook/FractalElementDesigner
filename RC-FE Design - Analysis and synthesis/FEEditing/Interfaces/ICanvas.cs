// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Interfaces
{
    public interface ICanvas
    {
        IEnumerable<IElement> GetElements();

        void Add(IElement element);
        void Remove(IElement element);
        void Clear();

        double GetWidth();
        void SetWidth(double width);
        double GetHeight();
        void SetHeight(double height);
    }
}
