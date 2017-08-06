using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hornet.ViewModel.ViewModel
{
    public class WindowCreatingViewModel : ViewModelBase
    {
        protected Window _activeWindow;

        public virtual void ShowWindow(bool dialog = false)
        {
            WindowMediator.RequestWindow(this, dialog);
        }

        protected virtual void CloseWindow()
        {
            _activeWindow?.Close();
        }

        internal void SetActiveWindow(Window view)
        {
            _activeWindow = view;
        }
    }
}
