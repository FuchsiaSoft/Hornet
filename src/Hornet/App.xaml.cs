using Hornet.DatabaseManagement;
using Hornet.ViewModel.ViewModel;
using Hornet.ViewModel.ViewModel.DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Hornet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected async override void OnStartup(StartupEventArgs e)
        {
            SplashScreen splash = new SplashScreen("Splash.png");
            splash.Show(false);
            await Task.Run(() =>
            {
                WindowMediator.StartListening();
                WindowMediator.Register<AddEditHashSetViewModel, AddEditHashSet>();
                WindowMediator.Register<AddEditRegexSetViewModel, AddEditRegexSet>();
                WindowMediator.Register<ProgressViewModel, ProgressWindow>();
            });
            Thread.Sleep(3000);
            splash.Close(TimeSpan.FromMilliseconds(5));
        }
    }
}
