﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FractalElementDesigner.SchemeEditing.Core;
using FractalElementDesigner.SchemeEditing.Core.Model;
using FractalElementDesigner.SchemeEditing.Editor;
using FractalElementDesigner.SchemeEditing.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes; 
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FractalElementDesigner.SchemeEditing
{
    using FactoryFunc = Func<object[], double, double, bool, object>;

    public class WpfSchemeCreator : ISchemeCreator
    {
        public WpfSchemeCreator()
        {
            InitializeFactory();
        }

        public Action<ElementThumb> SetThumbEvents { get; set; }
        public Action<IElement, double, double, bool> SetPosition { get; set; }     
        public Func<IdCounter> GetCounter { get; set; }
        private ICanvas ParserCanvas { get; set; }

        #region Factory

        private Dictionary<string, FactoryFunc> Factory { get; set; }

        private void InitializeFactory()
        {
            Factory = new Dictionary<string, FactoryFunc>()
            {
                {  Constants.TagElementPin, CreatePin },
                {  Constants.TagElementWire, CreateWire },
                {  Constants.TagElementFElement, CreateFElement },
                {  Constants.TagElementTopGround, CreateTopGround },
                {  Constants.TagElementBottomGround, CreateBottomGround },
            };
        }

        private object CreatePin(object[] data, double x, double y, bool snap)
        {
            if (data == null || data.Length != 1)
                return null;

            int id = (int)data[0];

            var thumb = new ElementThumb()
            {
                Template = Application.Current.Resources[ResourceConstants.KeyTemplatePin] as ControlTemplate,
                Style = Application.Current.Resources[ResourceConstants.KeySyleRootThumb] as Style,
                Uid = Constants.TagElementPin + Constants.TagNameSeparator + id.ToString()
            };

            SetThumbEvents(thumb);
            SetPosition(thumb, x, y, snap);

            return thumb;
        }

        private object CreateWire(object[] data, double x, double y, bool snap)
        {
            if (data == null || data.Length != 9)
                return null;

            double x1 = (double)data[0];
            double y1 = (double)data[1];
            double x2 = (double)data[2];
            double y2 = (double)data[3];
            bool startVisible = (bool)data[4];
            bool endVisible = (bool)data[5];
            bool startIsIO = (bool)data[6];
            bool endIsIO = (bool)data[7];
            int id = (int)data[8];

            var line = new LineEx()
            {
                Style = Application.Current.Resources[ResourceConstants.KeyStyleWireLine] as Style,
                X1 = 0, //X1 = x1,
                Y1 = 0, //Y1 = y1,
                Margin = new Thickness(x1, y1, 0, 0),
                X2 = x2 - x1, // X2 = x2,
                Y2 = y2 - y1, // Y2 = y2,
                IsStartVisible = startVisible,
                IsEndVisible = endVisible,
                IsStartIO = startIsIO,
                IsEndIO = endIsIO,
                Uid = Constants.TagElementWire + Constants.TagNameSeparator + id.ToString()
            };

            return line;
        }


        private object CreateFElement(object[] data, double x, double y, bool snap)
        {
            if (data == null || data.Length != 1)
                return null;

            int id = (int)data[0];

            var thumb = new ElementThumb()
            {
                ElementType = ElementType.SchemeElement,
                Template = Application.Current.Resources[ResourceConstants.KeyTemplateFElement] as ControlTemplate,
                Style = Application.Current.Resources[ResourceConstants.KeySyleRootThumb] as Style,
                Uid = Constants.TagElementFElement + Constants.TagNameSeparator + id.ToString()
            };

            SetThumbEvents(thumb);
            SetPosition(thumb, x, y, snap);

            return thumb;
        }

        private object CreateTopGround(object[] data, double x, double y, bool snap)
        {
            if (data == null || data.Length != 1)
                return null;

            int id = (int)data[0];

            var thumb = new ElementThumb()
            {
                ElementType = ElementType.TopGround,
                Template = Application.Current.Resources[ResourceConstants.KeyTemplateTopGround] as ControlTemplate,
                Style = Application.Current.Resources[ResourceConstants.KeySyleRootThumb] as Style,
                Uid = Constants.TagElementTopGround + Constants.TagNameSeparator + id.ToString()
            };

            SetThumbEvents(thumb);
            SetPosition(thumb, x, y, snap);

            return thumb;
        }

        private object CreateBottomGround(object[] data, double x, double y, bool snap)
        {
            if (data == null || data.Length != 1)
                return null;

            int id = (int)data[0];

            var thumb = new ElementThumb()
            {
                ElementType = ElementType.BottomGround,
                Template = Application.Current.Resources[ResourceConstants.KeyTemplateBottomGround] as ControlTemplate,
                Style = Application.Current.Resources[ResourceConstants.KeySyleRootThumb] as Style,
                Uid = Constants.TagElementBottomGround + Constants.TagNameSeparator + id.ToString()
            };

            SetThumbEvents(thumb);
            SetPosition(thumb, x, y, snap);

            return thumb;
        }


        #endregion

        #region ISchemeCreator

        public void SetCanvas(ICanvas canvas)
        {
            this.ParserCanvas = canvas;
        }

        public ICanvas GetCanvas()
        {
            return this.ParserCanvas;
        }

        public object CreateElement(string type, object[] data, double x, double y, bool snap)
        {
            FactoryFunc func;
            bool result = Factory.TryGetValue(type, out func);
            if (result == true && func != null)
                return func(data, x, y, snap);

            return null;
        }

        public object CreateScheme(SchemeProperties properties)
        {

            if (ParserCanvas != null)
            {
                ParserCanvas.SetWidth(properties.PageWidth);
                ParserCanvas.SetHeight(properties.PageHeight);
            }

            return null;
        }

        #endregion
    }

}
