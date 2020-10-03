using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls.Dialogs;
using FractalElementDesigner.Navigating;
using FractalElementDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using ControlzEx.Theming;
using MahApps.Metro.Theming;

namespace FractalElementDesigner
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
            base.OnStartup(e);

            var theme = ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri("pack://application:,,,/RC-FE Design - Analysis and synthesis;component/Themes/MainTheme.xaml"), MahAppsLibraryThemeProvider.DefaultInstance));
            ThemeManager.Current.ChangeTheme(this, theme);

            // Установить культурный контекст
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var window = new MainWindow();

            Navigation.Service = window.MainFrame.NavigationService;
            
            window.DataContext = new MainViewModel(new ViewModelsResolver(DialogCoordinator.Instance));

            window.Show();

            DispatcherHelper.Initialize();
        }
    }
}
