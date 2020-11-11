using FractalElementDesigner.FEEditing.Model;
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
        // Стиль для схемы
        public Style FElementSchemeStyle { get; set; }
        // Стиль для оболочки конструкции
        public Style FElementStructureWrapperStyle { get; set; }
        // Стиль для конструкции
        public Style FElementStructureStyle { get; set; }
        // Стиль для графика
        public Style PRPlotStyle { get; set; }
        // Стиль для слоя
        public Style LayerStructureStyle { get; set; }
        // Стиль по-умолчанию
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
                else if (projectTreeItem is StructureInProjectTree)
                {
                    return FElementStructureWrapperStyle;
                }
                else if (projectTreeItem is RCStructureBase)
                {
                    return FElementStructureStyle;
                }
                else if (projectTreeItem is Layer)
                {
                    return LayerStructureStyle;
                }
            }

            return DefaultStyle;
        }
    }
}
