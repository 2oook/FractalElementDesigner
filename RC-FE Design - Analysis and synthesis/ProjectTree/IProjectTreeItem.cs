using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.ProjectTree
{
    /// <summary>
    /// Интерфейс определяющий элемент дерева проекта
    /// </summary>
    public interface IProjectTreeItem
    {
        string Name { get; set; }
    }
}
