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
using System.Windows.Forms;
using Hornet.IO;
using System.Security;
using System.Net;

namespace Hornet.ViewModel.ViewModel
{
    public class MainViewModel : WindowCreatingViewModel
    {
        #region Binding Properties


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


        private string _RootDir;

        public string RootDir
        {
            get { return _RootDir; }
            set
            {
                _RootDir = value;
                RaisePropertyChanged("RootDir");
            }
        }


        private string _Domain;

        public string Domain
        {
            get { return _Domain; }
            set
            {
                _Domain = value;
                RaisePropertyChanged("Domain");
            }
        }

        private string _Username;

        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                RaisePropertyChanged("Username");
            }
        }


        private bool _HashJpegsInPdfs;

        public bool HashJpegsInPdfs
        {
            get { return _HashJpegsInPdfs; }
            set
            {
                _HashJpegsInPdfs = value;
                RaisePropertyChanged("HashJpegsInPdfs");
            }
        }

        private bool _HashFilesInOpenXml;

        public bool HashFilesInOpenXml
        {
            get { return _HashFilesInOpenXml; }
            set
            {
                _HashFilesInOpenXml = value;
                RaisePropertyChanged("HashFilesInOpenXml");
            }
        }

        private bool _HashFilesInZips;

        public bool HashFilesInZips
        {
            get { return _HashFilesInZips; }
            set
            {
                _HashFilesInZips = value;
                RaisePropertyChanged("HashFilesInZips");
            }
        }

        private bool _HashMsgAttachments;

        public bool HashMsgAttachments
        {
            get { return _HashMsgAttachments; }
            set
            {
                _HashMsgAttachments = value;
                RaisePropertyChanged("HashMsgAttachments");
            }
        }


        #endregion


        public MainViewModel()
        {
            if (IsInDesignMode) AddDesignTimeData();

        }
        
        public ICommand AddNewHashSetCommand { get { return new RelayCommand(NewHashSet); } }

        private void NewHashSet()
        {
            AddEditHashSetViewModel viewModel = new AddEditHashSetViewModel();
            viewModel.ShowWindow(false);
        }

        public ICommand AddNewRegexSetCommand { get { return new RelayCommand(NewRegexSet); } }

        private void NewRegexSet()
        {
            AddEditRegexSetViewModel viewModel = new AddEditRegexSetViewModel();
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

        public async void HandleDroppedRegexsetFiles(string[] files)
        {
            MarkBusy("Processing regex set files...");

            foreach (string filePath in files)
            {
                RegexInfoGroup group = await Task.Run(() =>
                {
                    try
                    {
                        RegexInfoGroup regexInfoGroup = RegexInfoGroup.FromFile(filePath);
                        return regexInfoGroup;
                    }
                    catch (Exception)
                    {
                        return null;
                        //TODO: error handling
                    }
                });

                if (group != null) AvailableRegexGroups.Add(group);
            }

            MarkFree();
        }

        public ICommand OpenHashSetCommand { get { return new RelayCommand(OpenHashSet); } }

        private async void OpenHashSet()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
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

        public ICommand OpenRegexSetCommand { get { return new RelayCommand(OpenRegexSet); } }

        private async void OpenRegexSet()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Choose regex set file";
            dlg.AddExtension = true;
            dlg.DefaultExt = ".rset";
            dlg.Filter = "Regex set definition (*.rset)|*.rset";
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == true)
            {
                RegexInfoGroup group = await Task.Run(() =>
                {
                    try
                    {
                        return RegexInfoGroup.FromFile(dlg.FileName);
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                });

                if (group != null)
                {
                    AddEditRegexSetViewModel viewModel = new AddEditRegexSetViewModel(group);
                    viewModel.ShowWindow();
                }
            }
        }

        public ICommand BrowseDirCommand { get { return new RelayCommand(BrowseDir); } }

        private void BrowseDir()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = "Choose a starting directory";
            dlg.ShowNewFolderButton = false;

            dlg.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dlg.SelectedPath))
            {
                RootDir = dlg.SelectedPath;
            }
        }

        
        public void StartScan(SecureString pwd)
        {
            //There is no ICommand for binding this method to the view,
            //because the view is using a password box with a secure string,
            //and there is no way to bind those controls, so this method is
            //invoked directly from the view.  Check code behind for the 
            //mainwindow.xaml.cs

            NetworkCredential credentials = new NetworkCredential()
            {
                Domain = Domain,
                SecurePassword = pwd,
                UserName = Username
            };

            ScanOptions options = new ScanOptions()
            {
                Credentials = credentials,
                RootDirectoryPath = RootDir,
                MaxWorkerThreads = 5,
                MaxSizeToAttemptHash = 2097152,
                MaxSizeToTextDecode = 2097152,
                AttemptTextDecode = false,
                HashIncludeZip = false
            };

            options.HashGroups.AddRange(AvailableHashGroups);
            options.RegexGroups.AddRange(AvailableRegexGroups);

            HornetScanManager.StartScanAsync(options);

            ProgressViewModel viewModel = new ProgressViewModel();
            viewModel.ShowWindow();

            CloseWindow();
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