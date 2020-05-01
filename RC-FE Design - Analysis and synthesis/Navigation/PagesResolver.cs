using RC_FE_Design___Analysis_and_synthesis.Navigation.Interfaces;
using RC_FE_Design___Analysis_and_synthesis.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RC_FE_Design___Analysis_and_synthesis.Navigation
{
    public class PagesResolver : IPageResolver
    {

        private readonly Dictionary<string, Func<Page>> _pagesResolvers = new Dictionary<string, Func<Page>>();

        public PagesResolver()
        {
            _pagesResolvers.Add(Navigation.MainPageAlias, () => new MainPage());
            _pagesResolvers.Add(Navigation.Page2Alias, () => new Page2());
            _pagesResolvers.Add(Navigation.Page3Alias, () => new Page3());
            _pagesResolvers.Add(Navigation.NotFoundPageAlias, () => new Page404());
        }

        public Page GetPageInstance(string alias)
        {
            if (_pagesResolvers.ContainsKey(alias))
            {
                return _pagesResolvers[alias]();
            }

            return _pagesResolvers[Navigation.NotFoundPageAlias]();
        }
    }
}
