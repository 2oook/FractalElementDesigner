﻿using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using RC_FE_Design___Analysis_and_synthesis.MathModel;
using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RC_FE_Design___Analysis_and_synthesis.Controls.Selectors
{
    /// <summary>
    /// Селектор шаблона узла дерева проекта
    /// </summary>
    public class ProjectTreeItemStyleSelector : DataTemplateSelector
    {
        public DataTemplate FElementSchemeDataTemplate { get; set; }
        public DataTemplate FElementStructureDataTemplate { get; set; }

        // Метод для переключения шаблона узла дерева проекта
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IProjectTreeItem projectTreeItem)
            {
                if (projectTreeItem is FElementScheme)
                {
                    return FElementSchemeDataTemplate;
                }
                else if (projectTreeItem is RCStructureBase)
                {
                    return FElementStructureDataTemplate;
                }
            }

            return null;
        }
    }
}
