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

        private string _WindowTitle;

        public string WindowTitle
        {
            get { return _WindowTitle; }
            set
            {
                _WindowTitle = value;
                RaisePropertyChanged("WindowTitle");
            }
        }


        private bool _IsBusy;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        private bool _EnableControls;

        public bool EnableControls
        {
            get { return _EnableControls; }
            set
            {
                _EnableControls = value;
                RaisePropertyChanged("EnableControls");
            }
        }

        private string _BusyMessage;

        public string BusyMessage
        {
            get { return _BusyMessage; }
            set
            {
                _BusyMessage = value;
                RaisePropertyChanged("BusyMessage");
            }
        }

        private string _ErrorMessage;

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

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

        protected void MarkBusy()
        {
            MarkBusy(string.Empty);
        }

        protected void MarkBusy(string busyMessage)
        {
            IsBusy = true;
            EnableControls = false;
            BusyMessage = busyMessage;
        }

        protected void MarkFree()
        {
            IsBusy = false;
            EnableControls = true;
            BusyMessage = string.Empty;
        }
    }
}
