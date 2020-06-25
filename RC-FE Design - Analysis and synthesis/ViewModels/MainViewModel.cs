using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.Commands;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RC_FE_Design___Analysis_and_synthesis.ViewModels
{
    /// <summary>
    /// Главная ViewModel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="resolver">Ссылка на объект для определения ViewModel'ов</param>
        public MainViewModel(IViewModelsResolver resolver)
        {
            _resolver = resolver;

            // создать страницы
            _AnalysisAndSynthesisPage = new AnalysisAndSynthesisPage();
            _SchemeEditorPage = new SchemeEditorPage();
        
            _SchemeEditorPageViewModel = _resolver.GetViewModelInstance(SchemeEditorPageViewModelAlias);
            _AnalysisAndSynthesisPageViewModel = _resolver.GetViewModelInstance(AnalysisAndSynthesisPageViewModelAlias);

            _AnalysisAndSynthesisPageViewModel.SetPage(_AnalysisAndSynthesisPage);

            // Зарегистрировать статические команды
            CommandManager.RegisterClassCommandBinding(typeof(Page), new CommandBinding(StaticCommandContainer.GoToSchemeEditorPageCommand, GoToSchemeEditorPageCommandExecute));
            CommandManager.RegisterClassCommandBinding(typeof(Page), new CommandBinding(StaticCommandContainer.GoToAnalysisAndSynthesisPageCommand, GoToAnalysisAndSynthesisPageCommandExecute));

            // Перейти на главную страницу
            GoToAnalysisAndSynthesisPageCommandExecute(null, null); 

            //GoToSchemeEditorPageCommandExecute(null, null); // ОТЛАДКА
        }

        #region Константы

        public static readonly string SchemeEditorPageViewModelAlias = "SchemeEditorPageVM";
        public static readonly string AnalysisAndSynthesisPageViewModelAlias = "AnalysisAndSynthesisPageVM";

        #endregion

        #region Глобальные переменные

        /// <summary>
        /// Ссылка на объект для разрешения ViewModel'ов
        /// </summary>
        private readonly IViewModelsResolver _resolver;

        /// <summary>
        /// Ссылка на ViewModel страницы редактора схем
        /// </summary>
        private readonly IPageViewModel _SchemeEditorPageViewModel;
        /// <summary>
        /// Ссылка на ViewModel страницы анализа и синтеза
        /// </summary>
        private readonly IPageViewModel _AnalysisAndSynthesisPageViewModel;

        /// <summary>
        /// Страница анализа
        /// </summary>
        private readonly Page _AnalysisAndSynthesisPage;
        /// <summary>
        /// Страница редактора схем
        /// </summary>
        private readonly Page _SchemeEditorPage;

        #endregion

        #region Свойства



        #endregion

        #region Команды



        #endregion

        #region Методы

        /// <summary>
        /// Метод для перемещения на страницу редактора схем
        /// </summary>
        private void GoToSchemeEditorPageCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Navigation.Navigation.Navigate(_SchemeEditorPage, _SchemeEditorPageViewModel);
        }

        /// <summary>
        /// Метод для перемещения на страницу анализа и синтеза
        /// </summary>
        private void GoToAnalysisAndSynthesisPageCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Navigation.Navigation.Navigate(_AnalysisAndSynthesisPage, _AnalysisAndSynthesisPageViewModel);
        }

        #endregion
    }
}
