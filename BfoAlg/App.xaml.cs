using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BfoAlg.ViewModels;

namespace BfoAlg
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Window window = new MainView();
            window.DataContext = new MainViewModel();
            window.Show();

            base.OnStartup(e);
        }
    }
}