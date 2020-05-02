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
    /// <summary>
    /// Класс для определения страниц
    /// </summary>
    public class PagesResolver : IPageResolver
    {
        /// <summary>
        /// Словарь страниц
        /// </summary>
        private readonly Dictionary<string, Func<Page>> _pagesResolvers = new Dictionary<string, Func<Page>>();

        /// <summary>
        /// Конструктор
        /// </summary>
        public PagesResolver()
        {
            _pagesResolvers.Add(Navigation.MainPageAlias, () => new MainPage());
            _pagesResolvers.Add(Navigation.AnalysisPageAlias, () => new AnalysisPage());
            _pagesResolvers.Add(Navigation.SynthesisPageAlias, () => new SynthesisPage());
            _pagesResolvers.Add(Navigation.NotFoundPageAlias, () => new Page404());
        }

        /// <summary>
        /// Метод для получения страницы
        /// </summary>
        /// <param name="alias">Псевдоним страницы</param>
        /// <returns>Ссылка на страницу</returns>
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
