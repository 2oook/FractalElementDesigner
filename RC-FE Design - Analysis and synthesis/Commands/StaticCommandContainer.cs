using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FractalElementDesigner.Commands
{
    /// <summary>
    /// Класс контейнер для статических команд
    /// </summary>
    public static class StaticCommandContainer
    {

        private static readonly RoutedUICommand goToStructureDesigningPageCommand = new RoutedUICommand("goToStructureDesigningPageCommand", "GoToStructureDesigningPageCommand", typeof(StaticCommandContainer));
        /// <summary>
        /// Команда переключения на страницу проектирования структуры
        /// </summary>
        public static RoutedUICommand GoToStructureDesigningPageCommand
        {
            get => goToStructureDesigningPageCommand;
        }
    }
}
