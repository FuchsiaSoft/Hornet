using GalaSoft.MvvmLight;
using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Hornet.ViewModel.ViewModel.DatabaseManagement;
using Microsoft.Win32;

namespace Hornet.ViewModel.ViewModel
{
    public class MainViewModel : WindowCreatingViewModel
    {
        #region Binding Properties

        //TODO: bind this somewhere in the main view
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


        private ObservableCollection<HashInfoGroup> _AvailableHashGroups
            = new ObservableCollection<HashInfoGroup>();

        public ObservableCollection<HashInfoGroup> AvailableHashGroups
        {
            get { return _AvailableHashGroups; }
            set
            {
                _AvailableHashGroups = value;
                RaisePropertyChanged("AvailableHashGroups");
            }
        }

        private ObservableCollection<HashInfoGroup> _SelectedHashGroups
            = new ObservableCollection<HashInfoGroup>();

        public ObservableCollection<HashInfoGroup> SelectedHashGroups
        {
            get { return _SelectedHashGroups; }
            set
            {
                _SelectedHashGroups = value;
                RaisePropertyChanged("SelectedHashGroups");
            }
        }


        private ObservableCollection<RegexInfoGroup> _AvailableRegexGroups
            = new ObservableCollection<RegexInfoGroup>();

        public ObservableCollection<RegexInfoGroup> AvailableRegexGroups
        {
            get { return _AvailableRegexGroups; }
            set
            {
                _AvailableRegexGroups = value;
                RaisePropertyChanged("AvailableRegexGroups");
            }
        }


        private ObservableCollection<RegexInfoGroup> _SelectedRegexGroups
            = new ObservableCollection<RegexInfoGroup>();

        public ObservableCollection<RegexInfoGroup> SelectedRegexGroups
        {
            get { return _SelectedRegexGroups; }
            set
            {
                _SelectedRegexGroups = value;
                RaisePropertyChanged("SelectedRegexGroups");
            }
        }


        #endregion


        public MainViewModel()
        {
            if (IsInDesignMode) AddDesignTimeData();

        }
        

      
        public ICommand AddNewHashSetCommand { get { return new RelayCommand(NewHashSet, CanNewHashSet); } }

        private bool CanNewHashSet()
        {
            return true;
        }

        private void NewHashSet()
        {
            AddEditHashSetViewModel viewModel = new AddEditHashSetViewModel();
            viewModel.ShowWindow(false);
        }

        public async void HandleDroppedHashsetFiles(string[] files)
        {
            MarkBusy("Processing hash set files...");

            foreach (string filePath in files)
            {
                HashInfoGroup group = await Task.Run(() =>
                {
                    try
                    {
                        HashInfoGroup hashInfoGroup = HashInfoGroup.FromFile(filePath);
                        return hashInfoGroup;
                    }
                    catch (Exception)
                    {
                        return null;
                        //TODO: error handling
                    }
                });

                if (group != null) AvailableHashGroups.Add(group);
            }

            MarkFree();
        }

        public ICommand OpenHashSetCommand { get { return new RelayCommand(OpenHashSet); } }

        private async void OpenHashSet()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Choose hash set file";
            dlg.AddExtension = true;
            dlg.DefaultExt = ".hset";
            dlg.Filter = "Hash set definition (*.hset)|*.hset";
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == true)
            {
                HashInfoGroup group = await Task.Run(() =>
                {
                    try
                    {
                        return HashInfoGroup.FromFile(dlg.FileName);
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                });

                if (group != null)
                {
                    AddEditHashSetViewModel viewModel = new AddEditHashSetViewModel(group);
                    viewModel.ShowWindow();
                }
            }
        }

        #region Design time data

        private void AddDesignTimeData()
        {
            AvailableHashGroups.Add(GetRandomHashGroup());
            AvailableHashGroups.Add(GetRandomHashGroup());
            AvailableHashGroups.Add(GetRandomHashGroup());

            SelectedHashGroups.Add(GetRandomHashGroup());
        }

        private static Random _random = new Random();

        private HashInfoGroup GetRandomHashGroup()
        {
            HashInfoGroup hashGroup = new HashInfoGroup()
            {
                Name = "An Example Hash Group, name goes here",
                Description = "This is an example hash group, its full description would go here"
            };

            int md5Count = _random.Next(3000);
            for (int i = 0; i < md5Count; i++)
            {
                hashGroup.MD5s.Add(new HashInfo()
                {
                    Hash = GetRandomMD5(),
                    HashType = HashType.MD5,
                    Remarks = "remarks go here about the file that made this hash"
                });
            }

            int sha1Count = _random.Next(2000);
            for (int i = 0; i < sha1Count; i++)
            {
                hashGroup.SHA1s.Add(new HashInfo()
                {
                    Hash = GetRandomSHA1(),
                    HashType = HashType.SHA1,
                    Remarks = "remarks go here about the file that made this hash"
                });
            }

            int sha256Count = _random.Next(1000);
            for (int i = 0; i < sha256Count; i++)
            {
                hashGroup.SHA256s.Add(new HashInfo()
                {
                    Hash = GetRandomSHA256(),
                    HashType = HashType.SHA256,
                    Remarks = "remarks go here about the file that made this hash"
                });
            }

            return hashGroup;
        }


        private static string GetRandomMD5()
        {
            byte[] buffer = new byte[10];
            _random.NextBytes(buffer);
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static string GetRandomSHA1()
        {
            byte[] buffer = new byte[10];
            _random.NextBytes(buffer);
            using (System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static string GetRandomSHA256()
        {
            byte[] buffer = new byte[10];
            _random.NextBytes(buffer);
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        #endregion
    }
}