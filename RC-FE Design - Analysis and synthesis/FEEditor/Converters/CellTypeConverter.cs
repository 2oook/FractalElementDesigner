﻿using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model.Cells;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Converters
{
    /// <summary>
    /// Класс конвертер для преобразования типа ячейки в цвет фона 
    /// </summary>
    public class CellTypeConverter : IValueConverter
    {
        /// <summary>
        /// Метод для конвертирования типа ячейки в цвет фона
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="targetType">Целевой тип</param>
        /// <param name="parameter">Параметр</param>
        /// <param name="culture">Культурный контекст</param>
        /// <returns>Цвет фона</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CellType cellType)
            {
                switch (cellType)
                {
                    case CellType.None:
                        return Application.Current.FindResource("NoneColorKey");
                        break;
                    case CellType.PlaceForContact:
                        return Application.Current.FindResource("PlaceForContactColorKey");
                        break;
                    case CellType.Contact:
                        return Application.Current.FindResource("ContactColorKey");
                        break;
                    case CellType.Cut:
                        return Application.Current.FindResource("CutColorKey");
                        break;
                    case CellType.Forbid:
                        return Application.Current.FindResource("ForbidColorKey");
                        break;
                    case CellType.RC:
                        return Application.Current.FindResource("RCColorKey");
                        break;
                    case CellType.R:
                        return Application.Current.FindResource("RColorKey");
                        break;
                    case CellType.Shunt:
                        return Application.Current.FindResource("ShuntColorKey");
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Метод для конвертирования фона в тип ячейки
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="targetType">Целевой тип</param>
        /// <param name="parameter">Параметр</param>
        /// <param name="culture">Культурный контекст</param>
        /// <returns>Тип ячейки</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
