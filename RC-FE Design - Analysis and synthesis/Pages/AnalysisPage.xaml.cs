using RC_FE_Design___Analysis_and_synthesis.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RC_FE_Design___Analysis_and_synthesis.Pages
{
    /// <summary>
    /// Interaction logic for AnalysisPage.xaml
    /// </summary>
    public partial class AnalysisPage : Page
    {
        public AnalysisPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик нажатия клавиш клавиатуры
        /// </summary>
        /// <param name="e">Параметры события</param>
        /// <param name="sender">Объект отправитель</param>
        private void HandleKeyEvents(object sender, KeyEventArgs e)
        {
            var context = this.DataContext as AnalysisViewModel;
            if (context == null) return;

            bool isControl = Keyboard.Modifiers == ModifierKeys.Control;
            var key = e.Key;

            if (isControl == true)
            {
                switch (key)
                {
                    case Key.O: break;
                    case Key.S: break;
                    case Key.N: context.NewStructureCommand.Execute(null); break;
                }
            }            
        }
    }
}
