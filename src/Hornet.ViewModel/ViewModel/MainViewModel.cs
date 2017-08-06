using GalaSoft.MvvmLight;
using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Hornet.ViewModel.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Binding Properties
        private ObservableCollection<HashInfoGroup> _AvailableHashInfoGroups 
            = new ObservableCollection<HashInfoGroup>();

        public ObservableCollection<HashInfoGroup> AvailableHashInfoGroups
        {
            get { return _AvailableHashInfoGroups; }
            set
            {
                _AvailableHashInfoGroups = value;
                RaisePropertyChanged("AvailableHashInfoGroups");
            }
        }


        private ObservableCollection<HashInfoGroup> _SelectedHashInfoGroups
            = new ObservableCollection<HashInfoGroup>();

        public ObservableCollection<HashInfoGroup> SelectedHashInfoGroups
        {
            get { return _SelectedHashInfoGroups; }
            set
            {
                _SelectedHashInfoGroups = value;
                RaisePropertyChanged("SelectedHashInfoGroups");
            }
        }


        private ObservableCollection<RegexInfoGroup> _AvailableRegexInfoGroups
            = new ObservableCollection<RegexInfoGroup>();

        public ObservableCollection<RegexInfoGroup> AvailableRegexInfoGroups
        {
            get { return _AvailableRegexInfoGroups; }
            set
            {
                _AvailableRegexInfoGroups = value;
                RaisePropertyChanged("AvailableRegexInfoGroups");
            }
        }


        private ObservableCollection<RegexInfoGroup> _SelectedRegexInfoGroups 
            = new ObservableCollection<RegexInfoGroup>();

        public ObservableCollection<RegexInfoGroup> SelectedRegexInfoGroups
        {
            get { return _SelectedRegexInfoGroups; }
            set
            {
                _SelectedRegexInfoGroups = value;
                RaisePropertyChanged("SelectedRegexInfoGroups");
            }
        }


        #endregion


        public MainViewModel()
        {
            if (IsInDesignMode) AddDesignTimeData();
            //TEMP remove this
            AddDesignTimeData();
        }

        private void AddDesignTimeData()
        {
            AvailableHashInfoGroups.Add(GetRandomHashGroup());
            AvailableHashInfoGroups.Add(GetRandomHashGroup());
            AvailableHashInfoGroups.Add(GetRandomHashGroup());

            SelectedHashInfoGroups.Add(GetRandomHashGroup());
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
            using (MD5 md5 = MD5.Create())
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
            using (SHA1 sha1 = SHA1.Create())
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
            using (SHA256 sha256 = SHA256.Create())
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
    }
}