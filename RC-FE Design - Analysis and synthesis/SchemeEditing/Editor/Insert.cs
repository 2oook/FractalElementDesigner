// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FractalElementDesigner.SchemeEditing.Core;
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

        public static IElement TopGround(ICanvas canvas, IPoint point, ISchemeCreator creator, bool snap)
        {
            var thumb = creator.CreateElement(Constants.TagElementTopGround,
                new object[] { canvas.GetCounter().Next() },
                point.X, point.Y, snap) as IThumb;

            canvas.Add(thumb);

            return thumb;
        }

        public static IElement BottomGround(ICanvas canvas, IPoint point, ISchemeCreator creator, bool snap)
        {
            var thumb = creator.CreateElement(Constants.TagElementBottomGround,
                new object[] { canvas.GetCounter().Next() },
                point.X, point.Y, snap) as IThumb;

            canvas.Add(thumb);

            return thumb;
        }

        public static IElement TopIn(ICanvas canvas, IPoint point, ISchemeCreator creator, bool snap)
        {
            var thumb = creator.CreateElement(Constants.TagElementTopIn,
                new object[] { canvas.GetCounter().Next() },
                point.X, point.Y, snap) as IThumb;

            canvas.Add(thumb);

            return thumb;
        }

        public static IElement BottomIn(ICanvas canvas, IPoint point, ISchemeCreator creator, bool snap)
        {
            var thumb = creator.CreateElement(Constants.TagElementBottomIn,
                new object[] { canvas.GetCounter().Next() },
                point.X, point.Y, snap) as IThumb;

            canvas.Add(thumb);

            return thumb;
        }

        public static IElement TopConn(ICanvas canvas, IPoint point, ISchemeCreator creator, bool snap)
        {
            var thumb = creator.CreateElement(Constants.TagElementTopConn,
                new object[] { canvas.GetCounter().Next() },
                point.X, point.Y, snap) as IThumb;

            canvas.Add(thumb);

            return thumb;
        }

        public static IElement BottomConn(ICanvas canvas, IPoint point, ISchemeCreator creator, bool snap)
        {
            var thumb = creator.CreateElement(Constants.TagElementBottomConn,
                new object[] { canvas.GetCounter().Next() },
                point.X, point.Y, snap) as IThumb;

            canvas.Add(thumb);

            return thumb;
        }
    }
}
