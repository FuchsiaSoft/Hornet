using GalaSoft.MvvmLight;
using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Hornet.Model;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;

namespace Hornet.ViewModel.ViewModel
{
    public class MainViewModel : ViewModelBase
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


        private ObservableCollection<HashGroup> _AvailableHashGroups
            = new ObservableCollection<HashGroup>();

        public ObservableCollection<HashGroup> AvailableHashGroups
        {
            get { return _AvailableHashGroups; }
            set
            {
                _AvailableHashGroups = value;
                RaisePropertyChanged("AvailableHashGroups");
            }
        }


        private ObservableCollection<HashGroup> _SelectedHashGroups
            = new ObservableCollection<HashGroup>();

        public ObservableCollection<HashGroup> SelectedHashGroups
        {
            get { return _SelectedHashGroups; }
            set
            {
                _SelectedHashGroups = value;
                RaisePropertyChanged("SelectedHashGroups");
            }
        }


        private ObservableCollection<RegexGroup> _AvailableRegexGroups
            = new ObservableCollection<RegexGroup>();

        public ObservableCollection<RegexGroup> AvailableRegexGroups
        {
            get { return _AvailableRegexGroups; }
            set
            {
                _AvailableRegexGroups = value;
                RaisePropertyChanged("AvailableRegexGroups");
            }
        }


        private ObservableCollection<RegexGroup> _SelectedRegexGroups
            = new ObservableCollection<RegexGroup>();

        public ObservableCollection<RegexGroup> SelectedRegexGroups
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

            LoadData();
        }

        private void LoadData()
        {
            LoadHashGroups();

            LoadRegexGroups();
        }

        private async void LoadRegexGroups()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (HornetModelContainer db = new HornetModelContainer())
                    {

                    }
                }
                catch (Exception)
                {
                    ErrorMessage = "Could not load data";
                }
            });
        }

        public async void LoadHashGroups()
        {
            List<HashGroup> groups = new List<HashGroup>();
            List<HashGroup> selectedGroups = new List<HashGroup>();

            await Task.Run(() =>
            {
                try
                {
                    using (HornetModelContainer db = new HornetModelContainer())
                    {
                        groups = db.HashGroups.ToList();

                        IEnumerable<int> previouslySelectedIDs = SelectedHashGroups.Select(g => g.Id);

                        IEnumerable<HashGroup> groupsToMove = groups.Where(g => previouslySelectedIDs.Contains(g.Id));

                        foreach (HashGroup group in groupsToMove)
                        {
                            selectedGroups.Add(group);
                            groups.Remove(group);
                        }
                    }
                }
                catch (Exception)
                {
                    ErrorMessage = "Could not load data";
                }
            });

            AvailableHashGroups = new ObservableCollection<HashGroup>(groups);
            SelectedHashGroups = new ObservableCollection<HashGroup>(selectedGroups);
        }

        private void AddDesignTimeData()
        {
            AvailableHashGroups.Add(GetRandomHashGroup());
            AvailableHashGroups.Add(GetRandomHashGroup());
            AvailableHashGroups.Add(GetRandomHashGroup());

            SelectedHashGroups.Add(GetRandomHashGroup());
        }

        private static Random _random = new Random();

        private HashGroup GetRandomHashGroup()
        {
            HashGroup hashGroup = new HashGroup()
            {
                Name = "An Example Hash Group, name goes here",
                Description = "This is an example hash group, its full description would go here"
            };

            int md5Count = _random.Next(3000);
            for (int i = 0; i < md5Count; i++)
            {
                hashGroup.HashEntries.Add(new Hornet.Model.MD5()
                {
                    HashValue = GetRandomMD5(),
                    Remarks = "remarks go here about the file that made this hash"
                });
            }

            int sha1Count = _random.Next(2000);
            for (int i = 0; i < sha1Count; i++)
            {
                hashGroup.HashEntries.Add(new Hornet.Model.SHA1()
                {
                    HashValue = GetRandomSHA1(),
                    Remarks = "remarks go here about the file that made this hash"
                });
            }

            int sha256Count = _random.Next(1000);
            for (int i = 0; i < sha256Count; i++)
            {
                hashGroup.HashEntries.Add(new Hornet.Model.SHA256()
                {
                    HashValue = GetRandomSHA256(),
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
    }
}