﻿using FractalElementDesigner.FEEditing.Model;
using FractalElementDesigner.MathModel;
using FractalElementDesigner.ProjectTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FractalElementDesigner.Controls.Selectors
{
    /// <summary>
    /// Селектор стиля узла дерева проекта
    /// </summary>
    public class ProjectTreeItemStyleSelector : StyleSelector
    {
        public Style FElementSchemeStyle { get; set; }
        public Style FElementStructureStyle { get; set; }
        public Style PRPlotStyle { get; set; }
        public Style DefaultStyle { get; set; }

        // Метод для переключения стиля узла дерева проекта
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is IProjectTreeItem projectTreeItem)
            {
                if (projectTreeItem is FElementScheme)
                {
                    return FElementSchemeStyle;
                }
                else if (projectTreeItem is PRPlot)
                {
                    return PRPlotStyle;
                }
                else if (projectTreeItem is RCStructureBase)
                {
                    return FElementStructureStyle;
                }
            }

            return DefaultStyle;
        }
    }
}
