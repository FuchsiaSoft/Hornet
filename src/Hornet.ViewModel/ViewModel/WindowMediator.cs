using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hornet.ViewModel.ViewModel
{
    public static class WindowMediator
    {
        public static bool IsListening { get; set; } = false;

        private static Dictionary<Type, Type> _register = new Dictionary<Type, Type>();

        public static void StartListening()
        {
            IsListening = true;
        }

        public static void Register<T1,T2>() 
            where T1 : WindowCreatingViewModel
            where T2 : Window
        {
            _register.Add(typeof(T1), typeof(T2));
        }

        public static void RequestWindow(WindowCreatingViewModel viewModel, bool dialog = false)
        {
            if (IsListening)
            {
                Type windowType = _register[viewModel.GetType()];
                Window view = (Window)Activator.CreateInstance(windowType);
                viewModel.SetActiveWindow(view);
                view.DataContext = viewModel;
                if (dialog)
                {
                    view.ShowDialog();
                }
                else
                {
                    view.Show();
                }
            }
        }
    }
}
