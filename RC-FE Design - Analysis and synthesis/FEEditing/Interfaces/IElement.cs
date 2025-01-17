﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Interfaces
{
    public interface IElement
    {
        double GetX();
        double GetY();
        void SetX(double x);
        void SetY(double y);

        object GetParent();
    }
}
