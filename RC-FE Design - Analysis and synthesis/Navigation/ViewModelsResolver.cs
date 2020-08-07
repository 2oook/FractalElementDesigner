using MahApps.Metro.Controls.Dialogs;
using FractalElementDesigner.Navigating.Interfaces;
using FractalElementDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalElementDesigner.Navigating
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
            _vmResolvers.Add(MainViewModel.SchemeEditorPageViewModelAlias, () => new SchemeEditorPageViewModel());
            _vmResolvers.Add(MainViewModel.StructureDesigningPageViewModelAlias, () => new StructureDesigningPageViewModel());
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dialogCoordinator">Объект для вывода диалогов</param>
        public ViewModelsResolver(IDialogCoordinator dialogCoordinator)
        {
            _vmResolvers.Add(MainViewModel.SchemeEditorPageViewModelAlias, () => new SchemeEditorPageViewModel(dialogCoordinator));
            _vmResolvers.Add(MainViewModel.StructureDesigningPageViewModelAlias, () => new StructureDesigningPageViewModel(dialogCoordinator));
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

            return null;
        }
    }
}
