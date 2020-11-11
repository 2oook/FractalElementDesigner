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
    /// Селектор шаблона узла дерева проекта
    /// </summary>
    public class ProjectTreeItemDataTemplateSelector : DataTemplateSelector
    {
        // Шаблон данных представляющий схему
        public DataTemplate FElementSchemeDataTemplate { get; set; }
        // Шаблон данных представляющий конструкцию
        public DataTemplate StructureInProjectTreeDataTemplate { get; set; }

        // Метод для переключения шаблона узла дерева проекта
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IProjectTreeItem projectTreeItem)
            {
                if (projectTreeItem is FElementScheme)
                {
                    return FElementSchemeDataTemplate;
                }
                else if (projectTreeItem is StructureInProjectTree)
                {
                    return StructureInProjectTreeDataTemplate;
                }
            }

            return null;
        }
    }
}
