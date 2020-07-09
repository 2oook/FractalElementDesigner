using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
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
    /// Селектор стиля узла дерева проекта
    /// </summary>
    public class ProjectTreeItemStyleSelector : StyleSelector
    {
        public Style FElementSchemeStyle { get; set; }
        public Style FElementStructureStyle { get; set; }
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
                else if (projectTreeItem is RCStructureBase)
                {
                    return FElementStructureStyle;
                }
            }

            return DefaultStyle;
        }
    }
}
