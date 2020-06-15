using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace RC_FE_Design___Analysis_and_synthesis.FEEditor.Tools
{
    /// <summary>
    /// Класс представляет инструмент для редактирования структуры
    /// </summary>
    public class Tool : ViewModelBase
    {
        /// <summary>
        /// Название свойства
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URI картинки инструмента
        /// </summary>
        public string ImageURI { get; set; } = string.Empty;

        /// <summary>
        /// Тип инструмента
        /// </summary>
        public ToolType Type { get; set; } = ToolType.None;

        private bool isChecked;
        /// <summary>
        /// Команда выбора инструмента редактирования
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                RaisePropertyChanged(nameof(IsChecked));
            }
        }
    }
}
