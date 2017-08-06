using GalaSoft.MvvmLight.CommandWpf;
using Hornet.IO.FileManagement;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hornet.ViewModel.ViewModel.DatabaseManagement
{
    public class AddEditHashSetViewModel : WindowCreatingViewModel
    {
        #region Binding Properties

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _Description;

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                RaisePropertyChanged("Description");
            }
        }


        private ObservableCollection<HashInfo> _MD5s = new ObservableCollection<HashInfo>();

        public ObservableCollection<HashInfo> MD5s
        {
            get { return _MD5s; }
            set
            {
                _MD5s = value;
                RaisePropertyChanged("MD5s");
            }
        }

        private ObservableCollection<HashInfo> _SHA1s = new ObservableCollection<HashInfo>();

        public ObservableCollection<HashInfo> SHA1s
        {
            get { return _SHA1s; }
            set
            {
                _SHA1s = value;
                RaisePropertyChanged("SHA1s");
            }
        }

        private ObservableCollection<HashInfo> _SHA256s = new ObservableCollection<HashInfo>();

        public ObservableCollection<HashInfo> SHA256s
        {
            get { return _SHA256s; }
            set
            {
                _SHA256s = value;
                RaisePropertyChanged("SHA256s");
            }
        }

        #endregion

        public ICommand CancelCommand { get { return new RelayCommand(CloseWindow); } }

        public ICommand SaveCommand { get { return new RelayCommand(Save); } }

        private async void Save()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.OverwritePrompt = true;
            dlg.AddExtension = true;
            dlg.DefaultExt = ".hg";
            if (dlg.ShowDialog() == true)
            {
                MarkBusy("Saving file...");
                await Task.Run(() =>
                {
                    try
                    {
                        HashInfoGroup group = new HashInfoGroup();
                        if (string.IsNullOrWhiteSpace(Name))
                        {
                            group.Name = "Un-named hash set";
                        }
                        else
                        {
                            group.Name = Name;
                        }

                        foreach (var md5 in MD5s)
                        {
                            md5.HashType = HashType.MD5;
                            group.MD5s.Add(md5);
                        }

                        foreach (var sha1 in SHA1s)
                        {
                            sha1.HashType = HashType.SHA1;
                            group.SHA1s.Add(sha1);
                        }

                        foreach (var sha256 in SHA256s)
                        {
                            sha256.HashType = HashType.SHA256;
                            group.SHA256s.Add(sha256);
                        }

                        //TODO: pick up here, saving a hg file!
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "Could not save file";
                    }
                    
                });
                MarkFree();
                
                
            }
        }
    }
}
