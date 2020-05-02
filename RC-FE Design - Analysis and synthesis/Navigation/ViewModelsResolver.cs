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
        private readonly Dictionary<string, Func<INotifyPropertyChanged>> _vmResolvers = new Dictionary<string, Func<INotifyPropertyChanged>>();

        /// <summary>
        /// Конструктор
        /// </summary>
        public ViewModelsResolver()
        {
            _vmResolvers.Add(MainViewModel.NotFoundPageViewModelAlias, () => new Page404ViewModel());
        }

        /// <summary>
        /// Метод для получения ViewModel'ов
        /// </summary>
        /// <param name="alias">Псевдоним страницы</param>
        /// <returns>ViewModel</returns>
        public INotifyPropertyChanged GetViewModelInstance(string alias)
        {
            if (_vmResolvers.ContainsKey(alias))
            {
                return _vmResolvers[alias]();
            }

            return _vmResolvers[MainViewModel.NotFoundPageViewModelAlias]();
        }
    }
}
