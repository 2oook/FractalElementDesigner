using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RC_FE_Design___Analysis_and_synthesis.Windows
{
    /// <summary>
    /// Interaction logic for StructureSchemeSynthesisParametersWindow.xaml
    /// </summary>
    public partial class StructureSchemeSynthesisParametersWindow : MetroWindow
    {
        public StructureSchemeSynthesisParametersWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Метод для вывода сообщения ошибки валидации
        /// </summary>
        internal void ShowValidationError()
        {
            this.ShowMessageAsync("Ошибка", "Введённые значения некорректны");
        }

        /// <summary>
        /// Метод для принятия корректного пользовательского ввода
        /// </summary>
        internal void AcceptUserInput()
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Обработчик нажатия кнопки Cancel
        /// </summary>
        /// <param name="sender">Объект источник</param>
        /// <param name="e">Параметры события</param>
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
