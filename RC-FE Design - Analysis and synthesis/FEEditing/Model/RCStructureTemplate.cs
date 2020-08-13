﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing.Model
{
    /// <summary>
    /// Класс шаблона RC структуры 
    /// </summary>
    class RCStructureTemplate
    {
        /// <summary>
        /// Название структуры
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Словарь свойств структуры
        /// </summary>
        public Dictionary<string, StructureProperty> StructureProperties { get; set; } = null;

        /// <summary>
        /// Словарь слоёв структуры
        /// </summary>
        public ObservableCollection<Layer> StructureLayers { get; set; } = null;
    }
}
