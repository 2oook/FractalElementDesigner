using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.Navigation
{
    /// <summary>
    /// Класс для определения ViewModel'ов 
    /// </summary>
    public class ViewModelsResolver : IViewModelsResolver
    {
        /// <summary>
        /// Словарь ViewModel'ов
        /// </summary>
        private readonly Dictionary<string, Func<IPageViewModel>> _vmResolvers = new Dictionary<string, Func<IPageViewModel>>();

        /// <summary>
        /// Конструктор
        /// </summary>
        public ViewModelsResolver()
        {
            _vmResolvers.Add(MainViewModel.NotFoundPageViewModelAlias, () => new Page404ViewModel());
            _vmResolvers.Add(MainViewModel.AnalysisPageViewModelAlias, () => new AnalysisViewModel());
            _vmResolvers.Add(MainViewModel.SynthesisPageViewModelAlias, () => new SynthesisViewModel());
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dialogCoordinator">Объект для вывода диалогов</param>
        public ViewModelsResolver(IDialogCoordinator dialogCoordinator)
        {
            _vmResolvers.Add(MainViewModel.NotFoundPageViewModelAlias, () => new Page404ViewModel());
            _vmResolvers.Add(MainViewModel.AnalysisPageViewModelAlias, () => new AnalysisViewModel(dialogCoordinator));
            _vmResolvers.Add(MainViewModel.SynthesisPageViewModelAlias, () => new SynthesisViewModel(dialogCoordinator));
        }

        /// <summary>
        /// Метод для получения ViewModel'ов
        /// </summary>
        /// <param name="alias">Псевдоним страницы</param>
        /// <returns>ViewModel</returns>
        public IPageViewModel GetViewModelInstance(string alias)
        {
            if (_vmResolvers.ContainsKey(alias))
            {
                return _vmResolvers[alias]();
            }

            return _vmResolvers[MainViewModel.NotFoundPageViewModelAlias]();
        }
    }
}
