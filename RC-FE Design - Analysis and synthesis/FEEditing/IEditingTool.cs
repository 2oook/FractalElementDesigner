using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.FEEditing
{
    interface IEditingTool : INotifyPropertyChanged
    {
        /// <summary>
        /// Название свойства
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// URI картинки инструмента
        /// </summary>
        string ImageURI { get; set; }

        /// <summary>
        /// Тип инструмента
        /// </summary>
        ToolType Type { get; set; }

        /// <summary>
        /// Флаг выбора инструмента редактирования
        /// </summary>
        bool IsChecked { get; set; }
    }
}
