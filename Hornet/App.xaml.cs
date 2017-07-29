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
        protected override void OnStartup(StartupEventArgs e)
        {
            SplashScreen splash = new SplashScreen("Splash.png");
            splash.Show(false);
            Thread.Sleep(7000);
            splash.Close(TimeSpan.FromMilliseconds(5));
        }
    }
}
