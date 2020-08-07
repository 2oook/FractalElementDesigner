using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.IO.ProjectSaveModel
{
    [Serializable]
    public class SavingProject
    {
        private string name = string.Empty;
        /// <summary>
        /// Название
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private ObservableCollection<SavingRCStructureBase> structures = new ObservableCollection<SavingRCStructureBase>();
        /// <summary>
        /// Список структур проекта
        /// </summary>
        public ObservableCollection<SavingRCStructureBase> Structures
        {
            get => structures;
            set => structures = value;
        }
    }
}
