using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Controls
{
    public class TableGrid : Grid
    {
        #region Data Attached Property

        public static object GetData(DependencyObject obj)
        {
            return (object)obj.GetValue(DataProperty);
        }

        public static void SetData(DependencyObject obj, object value)
        {
            obj.SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.RegisterAttached("Data", typeof(object), typeof(TableGrid),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion
    }
}
