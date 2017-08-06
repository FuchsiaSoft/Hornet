using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hornet.ViewModel.ViewModel.DatabaseManagement
{
    public enum DataEntryMode
    {
        New,
        Edit
    }

    /// <summary>
    /// Provides a base viewmodel for data entry screens, so that
    /// implementing a particular save/edit window can re-use the 
    /// same viewmodel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataEntryViewModelBase<T> : ViewModelBase
    {
        protected T _entity;
        protected DataEntryMode _mode;
        protected Action _exitAction;

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


        public DataEntryViewModelBase(T entity, DataEntryMode mode = DataEntryMode.New, Action exitAction)
        {
            _entity = entity;
            _mode = mode;

            DoLoading();
        }

        protected virtual async void DoLoading()
        {
            await LoadAlways();

            if (_mode == DataEntryMode.Edit)
            {
                await LoadExisting();
            }
            else
            {
                await LoadNew();
            }
        }

        protected abstract Task LoadAlways();

        protected abstract Task LoadExisting();

        protected abstract Task LoadNew();

        public RelayCommand<Window> CancelCommand { get { return new RelayCommand<Window>(Cancel, CanCancel); } }

        protected abstract bool CanCancel(Window window);

        protected virtual void Cancel(Window window)
        {
            CloseWindow(window);
        }

        protected virtual void CloseWindow(Window window)
        {
            window.Close();
        }

        public RelayCommand<Window> SaveCommand { get { return new RelayCommand<Window>(Save, CanSave); } }

        protected abstract bool CanSave(Window window);

        protected virtual async void Save(Window window)
        {
            if (_mode == DataEntryMode.Edit)
            {
                MarkBusy("Modifying data...");
                await SaveNew();
            }
            else
            {
                MarkBusy("Saving new data...");
                await SaveExisting();
            }

            MarkFree();
            CloseWindow(window);
            _exitAction.Invoke();
        }

        protected abstract Task SaveNew();

        protected abstract Task SaveExisting();

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
