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
        }

        // Метод для вставки соединений на схему
        public static void InsertConnections(FElementScheme scheme)
        {
            var sections = scheme.Editor.Context.CurrentCanvas.GetChildren();

            var sectionsCnt = sections.Count - 1;

            // обойти все соединения
            for (int i = 0; i < 3; i++)
            {
                var firstSectionChildren = FindVisualChild<Canvas>(sections[i] as DependencyObject);
                var firstSectionPins = firstSectionChildren.Children.OfType<PinThumb>().ToList();

                var secondSectionChildren = FindVisualChild<Canvas>(sections[i+1] as DependencyObject);
                var secondSectionPins = secondSectionChildren.Children.OfType<PinThumb>().ToList();

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

                            var first = currentFirstSectionPinList.SingleOrDefault(x => x.Name == MapPinNumberToString(k));
                            var second = currentSecondSectionPinList.SingleOrDefault(x => x.Name == MapPinNumberToString(j));

                            if (first == null | second == null)
                            {
                                throw new Exception("Вывод не найден!");
                            }
                            
                            scheme.Editor.Connect(scheme.Editor.Context.CurrentCanvas, first, scheme.Editor.Context.SchemeCreator);
                            scheme.Editor.Connect(scheme.Editor.Context.CurrentCanvas, second, scheme.Editor.Context.SchemeCreator);
                        }
                    }
                }
            }

            // отображение заземлений
            scheme.Editor.Add(scheme.Editor.Context.CurrentCanvas, Constants.TagElementTopGround, new PointEx(100, 100));

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
            string MapPinNumberToString(int pin)
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
