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
    /// Селектор шаблона данных внутри оболочки структуры в дереве проекта
    /// </summary>
    class StructureItemDataTemplateSelector : DataTemplateSelector
    {
        // Шаблон данных для структуры
        public DataTemplate LayersDataTemplate { get; set; }
        // Шаблон данных для графика
        public DataTemplate PlotDataTemplate { get; set; }

        // Метод для переключения шаблона внутри оболочки структуры
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IProjectTreeItem projectTreeItem)
            {
                if (projectTreeItem is PRPlot)
                {
                    return PlotDataTemplate;
                }
                else if (projectTreeItem is RCStructureBase)
                {
                    return LayersDataTemplate;
                }
            }

            return null;
        }
    }
}
