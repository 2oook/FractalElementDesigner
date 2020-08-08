﻿using FractalElementDesigner.SchemeEditing.Core;
using FractalElementDesigner.SchemeEditing.Core.Model;
using FractalElementDesigner.SchemeEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalElementDesigner.SchemeEditing.Editor
{
    public static class Insert
    {
        #region Pin

        public static IElement Pin(ICanvas canvas, IPoint point, ISchemeCreator creator, bool snap)
        {
            var thumb = creator.CreateElement(Constants.TagElementPin,
                new object[] { canvas.GetCounter().Next() },
                point.X, point.Y, snap) as IThumb;

            canvas.Add(thumb);

            return thumb;
        }

        #endregion


        #region FElement

        public static IElement FElement(ICanvas canvas, IPoint point, ISchemeCreator creator, bool snap)
        {
            var thumb = creator.CreateElement(Constants.TagElementFElement,
                new object[] { canvas.GetCounter().Next() },
                point.X, point.Y, snap) as IThumb;

            canvas.Add(thumb);

            return thumb;
        }


        #endregion
    }
}
