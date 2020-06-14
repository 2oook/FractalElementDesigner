using RC_FE_Design___Analysis_and_synthesis.FEEditor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.ProjectTree
{
    /// <summary>
    /// Проект
    /// </summary>
    public class Project
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

        private ObservableCollection<RCStructureBase> structures = new ObservableCollection<RCStructureBase>();
        /// <summary>
        /// Список структур проекта
        /// </summary>
        public ObservableCollection<RCStructureBase> Structures 
        { 
            get => structures; 
            set => structures = value; 
        }
    }
}
