/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Hornet.ViewModel"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Hornet.ViewModel.ViewModel.DatabaseManagement;
using Microsoft.Practices.ServiceLocation;

namespace Hornet.ViewModel.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
        }

        private static MainViewModel _Main = new MainViewModel(); //only ever one of this

        public MainViewModel Main
        {
            get
            {
                return _Main;
            }
        }

        public AddEditHashSetViewModel AddHashSet
        {
            get
            {
                return new AddEditHashSetViewModel(null, DataEntryMode.New, new System.Action(_Main.LoadHashGroups));
            }
        }

        //TODO: here we will create new viewmodel for popup windows and pass
        //through delegate for main viewmodel refresh commands
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}