using RC_FE_Design___Analysis_and_synthesis.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RC_FE_Design___Analysis_and_synthesis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var window = new MainWindow();
            var mvm = new MainViewModel();
            window.DataContext = mvm;
            window.Show();
        }
    }
}
