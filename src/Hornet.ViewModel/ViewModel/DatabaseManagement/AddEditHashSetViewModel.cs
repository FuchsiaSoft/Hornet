using Hornet.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hornet.ViewModel.ViewModel.DatabaseManagement
{
    public class AddEditHashSetViewModel : DataEntryViewModelBase<HashGroup>
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


        private ObservableCollection<MD5> _MD5s = new ObservableCollection<MD5>();

        public ObservableCollection<MD5> MD5s
        {
            get { return _MD5s; }
            set
            {
                _MD5s = value;
                RaisePropertyChanged("MD5s");
            }
        }

        private ObservableCollection<SHA1> _SHA1s = new ObservableCollection<SHA1>();

        public ObservableCollection<SHA1> SHA1s
        {
            get { return _SHA1s; }
            set
            {
                _SHA1s = value;
                RaisePropertyChanged("SHA1s");
            }
        }

        private ObservableCollection<SHA256> _SHA256s = new ObservableCollection<SHA256>();

        public ObservableCollection<SHA256> SHA256s
        {
            get { return _SHA256s; }
            set
            {
                _SHA256s = value;
                RaisePropertyChanged("SHA256s");
            }
        }

        #endregion

        public AddEditHashSetViewModel(HashGroup entity, DataEntryMode mode = DataEntryMode.New, Action exitAction = null) : 
            base(entity, mode, exitAction)
        {
            if (mode == DataEntryMode.New)
            {
                WindowTitle = "Add new hash set";
            }
            else
            {
                WindowTitle = "Edit hash set";
            }
        }

        protected override bool CanCancel(Window window)
        {
            return true; //can always cancel this one
        }

        protected override bool CanSave(Window window)
        {
            return (MD5s.Count > 0 || SHA1s.Count > 0 || SHA256s.Count > 0);
        }

        protected override Task LoadExisting()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (HornetModelContainer db = new HornetModelContainer())
                    {
                        HashGroup group = db.HashGroups.Find(_entity.Id);

                        IEnumerable<MD5> md5s = group.HashEntries.OfType<MD5>().ToList();
                        IEnumerable<SHA1> sha1s = group.HashEntries.OfType<SHA1>().ToList();
                        IEnumerable<SHA256> sha256s = group.HashEntries.OfType<SHA256>().ToList();

                        MD5s = new ObservableCollection<MD5>(md5s);
                        SHA1s = new ObservableCollection<SHA1>(sha1s);
                        SHA256s = new ObservableCollection<SHA256>(sha256s);
                    }
                }
                catch (Exception)
                {
                    ErrorMessage = "Could not save record to database";
                }
            });
            
        }

        protected override Task LoadNew()
        {
            return Task.FromResult(0); //nothing to see here
        }

        protected override Task SaveExisting()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (HornetModelContainer db = new HornetModelContainer())
                    {
                        HashGroup group = db.HashGroups.Find(_entity.Id);

                        group.HashEntries.Clear();

                        foreach (MD5 md5 in MD5s)
                        {
                            group.HashEntries.Add(md5);
                        }

                        foreach (SHA1 sha1 in SHA1s)
                        {
                            group.HashEntries.Add(sha1);
                        }

                        foreach (SHA256 sha256 in SHA256s)
                        {
                            group.HashEntries.Add(sha256);
                        }

                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    ErrorMessage = "Could not save record to database";
                }
            });
        }

        protected override Task SaveNew()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (HornetModelContainer db = new HornetModelContainer())
                    {
                        HashGroup group = new HashGroup();
                        
                        foreach (MD5 md5 in MD5s)
                        {
                            group.MD5s.Add(md5);
                        }

                        foreach (SHA1 sha1 in SHA1s)
                        {
                            group.HashEntries.Add(sha1);
                        }

                        foreach (SHA256 sha256 in SHA256s)
                        {
                            group.HashEntries.Add(sha256);
                        }

                        db.HashGroups.Add(group);
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    ErrorMessage = "Could not save record to database";
                }
            });
        }
    }
}
