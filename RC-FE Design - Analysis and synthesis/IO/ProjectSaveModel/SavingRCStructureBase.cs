﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.IO.ProjectSaveModel
{
    [Serializable]
    public class SavingRCStructureBase
    {
        /// <summary>
        /// Название структуры
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Словарь слоёв структуры
        /// </summary>
        public ObservableCollection<SavingLayer> StructureLayers { get; set; } = new ObservableCollection<SavingLayer>();
    }
}
