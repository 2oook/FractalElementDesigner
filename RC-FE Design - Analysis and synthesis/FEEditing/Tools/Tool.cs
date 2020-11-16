using System.ComponentModel;

namespace FractalElementDesigner.FEEditing
{
    /// <summary>
    /// Класс представляет инструмент для редактирования структуры
    /// </summary>
    public class Tool : IEditingTool
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
        /// Флаг выбора инструмента редактирования
        /// </summary>
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                RaisePropertyChanged(nameof(IsChecked));
            }
        }

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
