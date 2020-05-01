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
    public class ViewModelsResolver : IViewModelsResolver
    {

        private readonly Dictionary<string, Func<INotifyPropertyChanged>> _vmResolvers = new Dictionary<string, Func<INotifyPropertyChanged>>();

        public ViewModelsResolver()
        {
            //_vmResolvers.Add(MainPageViewModel.MainPageViewModelAlias, () => new MainPageViewModel());
            _vmResolvers.Add(MainViewModel.NotFoundPageViewModelAlias, () => new Page404ViewModel());
        }

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
