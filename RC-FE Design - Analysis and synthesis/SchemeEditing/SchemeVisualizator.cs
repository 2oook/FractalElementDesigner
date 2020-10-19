using FractalElementDesigner.MathModel;
using FractalElementDesigner.SchemeEditing.Controls;
using FractalElementDesigner.SchemeEditing.Core;
using FractalElementDesigner.SchemeEditing.Editor;
using FractalElementDesigner.SchemeEditing.Elements;
using FractalElementDesigner.SchemeEditing.Views;
using MahApps.Metro.Controls;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FractalElementDesigner.SchemeEditing
{
    /// <summary>
    /// Класс для визуализации схемы
    /// </summary>
    class SchemeVisualizator
    {
        // Вставить секции на схему
        public static void InsertSections(FElementScheme scheme)
        {
            var elementHeight = (double)Application.Current.FindResource("FEElementHeightKey");
            var elementWidth = (double)Application.Current.FindResource("FEElementWidthKey");

            // расположить элементы равномерно по ширине и высоте листа 
            var sheetHeight = scheme.Editor.Context.CurrentCanvas.GetHeight();
            var startVerticalAxeCoordinate = sheetHeight / 2;
            var startVerticalCoordinate = startVerticalAxeCoordinate - (elementHeight / 2);

            // переделать на фиксированные значения ширины и высоты 
            var sheetWidth = scheme.Editor.Context.CurrentCanvas.GetWidth();
            var horizontalPartOfWidth = sheetWidth / scheme.Model.FESections.Count;
            var startHorizontalAxeCoordinate = horizontalPartOfWidth / 2;
            var startHorizontalCoordinate = startHorizontalAxeCoordinate - (elementWidth / 2);

            // отобразить секции
            for (int i = 0; i < scheme.Model.FESections.Count; i++)
            {
                var horizontalCoordinate = startHorizontalCoordinate + horizontalPartOfWidth * i;
                var addedElement = scheme.Editor.Add(scheme.Editor.Context.CurrentCanvas, Constants.TagElementFElement, new PointEx(horizontalCoordinate, startVerticalCoordinate));
            }

            InsertConnections(scheme);
        }

        // Метод для вставки соединений на схему
        public static void InsertConnections(FElementScheme scheme)
        {
            var sections = new List<System.Windows.UIElement>();

            foreach (var item in scheme.Editor.Context.CurrentCanvas.GetChildren())
            {
                sections.Add((System.Windows.UIElement)item);
            }

            var sectionsCnt = sections.Count - 1;

            // обойти все соединения
            for (int i = 0; i < 3; i++)
            {
                var firstSection = (FrameworkElement)sections[i];
                firstSection.Measure(new Size());

                var secondSection = (FrameworkElement)sections[i + 1];
                secondSection.Measure(new Size());

                // получить потомков первой секции и отфильтровать в них пины
                var firstSectionChildren = FindVisualChild<Canvas>(firstSection);
                var firstSectionPins = firstSectionChildren.Children.OfType<PinThumb>().ToList();
                // получить потомков второй секции и отфильтровать в них пины
                var secondSectionChildren = FindVisualChild<Canvas>(secondSection);
                var secondSectionPins = secondSectionChildren.Children.OfType<PinThumb>().ToList();

                // найти закодированное соединение и его матрицу
                var connection = scheme.Model.InnerConnections[i];
                var connectionMatrix = FElementScheme.AllowablePinsConnections[connection.ConnectionType];
                var upperBound0 = connectionMatrix.ConnectionMatrix.GetUpperBound(0);
                var upperBound1 = connectionMatrix.ConnectionMatrix.GetUpperBound(1);

                List<PinThumb> currentFirstSectionPinList = null;
                List<PinThumb> currentSecondSectionPinList = null;

                // обойти матрицу инцидентности (верхний треугольник)
                for (int k = 0; k <= upperBound0; k++)
                {
                    for (int j = k + 1; j <= upperBound1; j++)
                    {
                        if (connectionMatrix.ConnectionMatrix[k, j] == 1)
                        {
                            // исключить диагональ соединения выводов
                            if ((k == 0 & j == 2) | (k == 2 & j == 0) | (k == 1 & j == 3) | (k == 3 & j == 1))
                            {
                                continue;
                            }

                            // определить набор пинов секции для поиска вывода в зависимости от номера соединяемого вывода
                            if (MapPinNumberToSectionNumber(k) == 1)
                            {
                                currentFirstSectionPinList = firstSectionPins;
                            }
                            else
                            {
                                currentFirstSectionPinList = secondSectionPins;
                            }

                            if (MapPinNumberToSectionNumber(j) == 1)
                            {
                                currentSecondSectionPinList = firstSectionPins;
                            }
                            else
                            {
                                currentSecondSectionPinList = secondSectionPins;
                            }

                            var first = currentFirstSectionPinList.SingleOrDefault(x => x.Name == MapPinNumberToStringOnInnerConnections(k));
                            var second = currentSecondSectionPinList.SingleOrDefault(x => x.Name == MapPinNumberToStringOnInnerConnections(j));
                            
                            if (first == null | second == null)
                            {
                                throw new Exception("Вывод не найден!");
                            }
                            
                            scheme.Editor.Connect(scheme.Editor.Context.CurrentCanvas, first, scheme.Editor.Context.SchemeCreator);
                            scheme.Editor.Connect(scheme.Editor.Context.CurrentCanvas, second, scheme.Editor.Context.SchemeCreator);
                        }
                    }
                }

                // найти закодированный вектор заземлений
                var groundVector = FElementScheme.AllowablePinsConnections[connection.ConnectionType].PEVector[connection.PEType];
                // отображение заземлений
                for (int g = 0; g < groundVector.Length; g++)
                {
                    if (groundVector[g] == 1)
                    {
                        // определить набор пинов секции для поиска вывода в зависимости от номера соединяемого вывода
                        if (MapPinNumberToSectionNumber(g) == 1)
                        {
                            currentFirstSectionPinList = firstSectionPins;
                        }
                        else
                        {
                            currentFirstSectionPinList = secondSectionPins;
                        }

                        var pinOnElement = currentFirstSectionPinList.SingleOrDefault(p => p.Name == MapPinNumberToStringOnInnerConnections(g));

                        ElementThumb groundingSection = null;

                        if (MapPinNumberToSectionNumber(g) == 1)
                        {
                            groundingSection = (ElementThumb)firstSection;
                        }
                        else
                        {
                            groundingSection = (ElementThumb)secondSection;
                        }

                        var elementTag = string.Empty;

                        double x = 0;
                        double y = 0;

                        // расположить символы заземления в зависимости от того сверху или снизу они расположены
                        if (g == 0 | g == 1)
                        {
                            elementTag = Constants.TagElementTopGround;
                            x = groundingSection.GetX() + pinOnElement.GetX() - 15;
                            y = groundingSection.GetY() - pinOnElement.GetY() - 30;
                        }
                        else
                        {
                            elementTag = Constants.TagElementBottomGround;
                            x = groundingSection.GetX() + pinOnElement.GetX() - 15;
                            y = groundingSection.GetY() + pinOnElement.GetY() + 30;
                        }

                        var addedGround = scheme.Editor.Add(scheme.Editor.Context.CurrentCanvas, elementTag, new PointEx(x, y));
                        
                        ((FrameworkElement)addedGround).Measure(new Size());

                        var groundPin = FindVisualChild<PinThumb>(addedGround as DependencyObject);

                        scheme.Editor.Connect(scheme.Editor.Context.CurrentCanvas, pinOnElement, scheme.Editor.Context.SchemeCreator);
                        scheme.Editor.Connect(scheme.Editor.Context.CurrentCanvas, groundPin, scheme.Editor.Context.SchemeCreator);
                    }
                }
            }

            InsertOuterPinsStates(scheme, sections);

            // метод для определения номера секции (1я или вторая) по номеру вывода в соединении
            int MapPinNumberToSectionNumber(int pin)
            {
                switch (pin)
                {
                    case 1:
                        return 2;
                    case 0:
                        return 1;
                    case 3:
                        return 1;
                    case 2:
                        return 2;
                }

                return -1;
            }

            // метод для нахождения названия вывода по номеру вывода из типового соединения
            string MapPinNumberToStringOnInnerConnections(int pin)
            {
                switch (pin)
                {
                    case 1:
                        return "LeftTopPin";
                    case 0:
                        return "RightTopPin";
                    case 3:
                        return "RightBottomPin";
                    case 2:
                        return "LeftBottomPin";
                }
                
                return "";
            }
        }


        public static void InsertOuterPinsStates(FElementScheme scheme, List<System.Windows.UIElement> sections)
        {
            FrameworkElement section = null;

            var firstSection = (FrameworkElement)sections[0];
            var lastSection = (FrameworkElement)sections[sections.Count - 1];

            // получить потомков первой секции и отфильтровать в них пины
            var firstSectionChildren = FindVisualChild<Canvas>(firstSection);
            var firstSectionPins = firstSectionChildren.Children.OfType<PinThumb>().ToList();
            // получить потомков второй секции и отфильтровать в них пины
            var lastSectionChildren = FindVisualChild<Canvas>(lastSection);
            var lastSectionPins = lastSectionChildren.Children.OfType<PinThumb>().ToList();

            List<PinThumb> sectionPinList = null;

            for (int i = 0; i < scheme.Model.OuterPins.Count; i++)
            {
                var outer_pin = scheme.Model.OuterPins[i];

                if (outer_pin.State == OuterPinState.Float)
                {
                    continue;
                }  

                if (MapPinNumberToSection(i) == "first")
                {
                    sectionPinList = firstSectionPins;
                }
                else
                {
                    sectionPinList = lastSectionPins;
                }

                var pinOnElement = sectionPinList.SingleOrDefault(p => p.Name == MapPinNumberToStringOnOuterConnections(i));

                var elementTag = string.Empty;

                double x = 0;
                double y = 0;

                if (i == 0 || i == 3)
                {
                    section = firstSection;
                }
                else if (i == 1 || i == 2)
                {
                    section = lastSection;
                }

                if (i == 0)
                {
                    elementTag = SwitchFotTopElements(outer_pin.State);
                    x = ((ElementThumb)section).GetX() + pinOnElement.GetX() - 15;
                    y = ((ElementThumb)section).GetY() - pinOnElement.GetY() - 30;
                }
                else if (i == 1)
                {
                    elementTag = SwitchFotTopElements(outer_pin.State);
                    x = ((ElementThumb)section).GetX() + pinOnElement.GetX() - 15;
                    y = ((ElementThumb)section).GetY() - pinOnElement.GetY() - 30;
                }
                else if (i == 2)
                {
                    elementTag = SwitchFotBottomElements(outer_pin.State);
                    x = ((ElementThumb)section).GetX() + pinOnElement.GetX() - 15;
                    y = ((ElementThumb)section).GetY() + pinOnElement.GetY() + 30;
                }
                else if (i == 3)
                {
                    elementTag = SwitchFotBottomElements(outer_pin.State);
                    x = ((ElementThumb)section).GetX() + pinOnElement.GetX() - 15;
                    y = ((ElementThumb)section).GetY() + pinOnElement.GetY() + 30;
                }

                var addedElement = scheme.Editor.Add(scheme.Editor.Context.CurrentCanvas, elementTag, new PointEx(x, y));

                ((FrameworkElement)addedElement).Measure(new Size());

                var addedElementPin = FindVisualChild<PinThumb>(addedElement as DependencyObject);

                scheme.Editor.Connect(scheme.Editor.Context.CurrentCanvas, pinOnElement, scheme.Editor.Context.SchemeCreator);
                scheme.Editor.Connect(scheme.Editor.Context.CurrentCanvas, addedElementPin, scheme.Editor.Context.SchemeCreator);
            }

            string SwitchFotTopElements(OuterPinState state) 
            {
                var elementTag = string.Empty;

                switch (state)
                {
                    case OuterPinState.Gnd:
                        elementTag = Constants.TagElementTopGround;
                        break;
                    case OuterPinState.In:
                        elementTag = Constants.TagElementTopIn;
                        break;
                    case OuterPinState.Float:
                        break;
                    case OuterPinState.Con:
                        elementTag = Constants.TagElementTopConn;
                        break;
                }

                return elementTag;
            }

            string SwitchFotBottomElements(OuterPinState state)
            {
                var elementTag = string.Empty;

                switch (state)
                {
                    case OuterPinState.Gnd:
                        elementTag = Constants.TagElementBottomGround;
                        break;
                    case OuterPinState.In:
                        elementTag = Constants.TagElementBottomIn;
                        break;
                    case OuterPinState.Float:
                        break;
                    case OuterPinState.Con:
                        elementTag = Constants.TagElementBottomConn;
                        break;
                }

                return elementTag;
            }

            // метод для определения номера секции (1я или вторая) по номеру вывода в соединении
            string MapPinNumberToSection(int pin)
            {
                switch (pin)
                {
                    case 0:
                        return "first";
                    case 1:
                        return "last";
                    case 2:
                        return "last";
                    case 3:
                        return "first";
                }

                return "";
            }

            // метод для нахождения названия вывода по номеру вывода 
            string MapPinNumberToStringOnOuterConnections(int pin)
            {
                switch (pin)
                {
                    case 0:
                        return "LeftTopPin";
                    case 1:
                        return "RightTopPin";
                    case 2:
                        return "RightBottomPin";
                    case 3:
                        return "LeftBottomPin";
                }

                return "";
            }
        }

        // Метод для поиска визуального потомка элемента
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
    }
}
