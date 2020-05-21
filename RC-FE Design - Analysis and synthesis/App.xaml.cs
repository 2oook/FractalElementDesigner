using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls.Dialogs;
using RC_FE_Design___Analysis_and_synthesis.Navigation;
using RC_FE_Design___Analysis_and_synthesis.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace RC_FE_Design___Analysis_and_synthesis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Обработчик старта приложения
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Установить культурный контекст
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var window = new MainWindow();

            Navigation.Navigation.Service = window.MainFrame.NavigationService;
            
            window.DataContext = new MainViewModel(new ViewModelsResolver(DialogCoordinator.Instance));

            window.Show();

            DispatcherHelper.Initialize();
        }
    }
}
