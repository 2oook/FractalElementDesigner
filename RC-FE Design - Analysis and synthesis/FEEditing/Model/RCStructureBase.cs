﻿using GalaSoft.MvvmLight;
using FractalElementDesigner.ProjectTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FractalElementDesigner.FEEditing.Model
{
    /// <summary>
    /// Базовый класс структуры 
    /// </summary>
    class RCStructureBase : INotifyPropertyChanged, IProjectTreeItem
    {
        /// <summary>
        /// Порядковый номер структуры
        /// </summary>
        private static int StructureCurrentNumber { get; set; } = 0;

        /// <summary>
        /// Метод для получения следующего номера структуры
        /// </summary>
        /// <returns>Номер структуры</returns>
        protected static int GetNextStructureNumber() 
        {
            return StructureCurrentNumber++;
        }

        /// <summary>
        /// Название структуры
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Словарь слоёв структуры
        /// </summary>
        public ObservableCollection<Layer> StructureLayers { get; set; } = new ObservableCollection<Layer>();

        /// <summary>
        /// Точки функции ФЧХ
        /// </summary>
        [NonSerialized]
        public List<(double frequency, double phase)> PhaseResponsePoints;

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для поднятия события изменения свойства
        /// </summary>
        /// <param name="propName">Имя свойства</param>
        protected virtual void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
