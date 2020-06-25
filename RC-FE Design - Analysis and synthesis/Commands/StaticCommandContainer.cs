using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.Commands
{
    /// <summary>
    /// Класс контейнер для статических команд
    /// </summary>
    public static class StaticCommandContainer
    {
        private static readonly RoutedUICommand goToSchemeEditorPageCommand = new RoutedUICommand("goToSchemeEditorPageCommand", "GoToSchemeEditorPageCommand", typeof(StaticCommandContainer));
        /// <summary>
        /// Команда переключения на страницу редактора схем
        /// </summary>
        public static RoutedUICommand GoToSchemeEditorPageCommand
        {
            get => goToSchemeEditorPageCommand;
        }

        private static readonly RoutedUICommand goToAnalysisAndSynthesisPageCommand = new RoutedUICommand("goToAnalysisAndSynthesisPageCommand", "GoToAnalysisAndSynthesisPageCommand", typeof(StaticCommandContainer));
        /// <summary>
        /// Команда переключения на страницу анализа и синтеза
        /// </summary>
        public static RoutedUICommand GoToAnalysisAndSynthesisPageCommand
        {
            get => goToAnalysisAndSynthesisPageCommand;
        }
    }
}
