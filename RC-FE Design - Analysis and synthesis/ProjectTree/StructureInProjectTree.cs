using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.ProjectTree
{
    /// <summary>
    /// Класс представляет структуру в дереве проекта
    /// </summary>
    class StructureInProjectTree : IProjectTreeItem
    {
        private string name;
        /// <summary>
        /// Название
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private ObservableCollection<IProjectTreeItem> items = new ObservableCollection<IProjectTreeItem>();
        /// <summary>
        /// Список элементов 
        /// </summary>
        public ObservableCollection<IProjectTreeItem> Items
        {
            get => items;
            set => items = value;
        }
    }
}
