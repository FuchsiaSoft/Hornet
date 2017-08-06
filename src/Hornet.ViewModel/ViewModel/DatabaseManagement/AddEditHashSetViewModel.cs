using GalaSoft.MvvmLight.CommandWpf;
using Hornet.IO;
using Hornet.IO.FileManagement;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            dlg.Title = "Choose location to save hash set file";
            dlg.OverwritePrompt = true;
            dlg.AddExtension = true;
            dlg.DefaultExt = ".hset";
            dlg.Filter = "Hash set definition (*.hset)|*.hset";
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

                        group.SaveToFile(dlg.FileName);
                    }
                    catch (Exception)
                    {
                        //TODO: better exception handling
                        ErrorMessage = "Could not save file";
                    }
                    
                });
                MarkFree();

                CloseWindow();
            }
        }

        public async void HandleDroppedFiles(string[] files)
        {
            MarkBusy("Processing files...");

            List<string> md5Matches = new List<string>();
            List<string> sha1Matches = new List<string>();
            List<string> sha256Matches = new List<string>();

            Regex md5Regex = new Regex(@"\b([A-F0-9]{32})\b", RegexOptions.Compiled);
            Regex sha1Regex = new Regex(@"\b([A-F0-9]{40})\b", RegexOptions.Compiled);
            Regex sha256Regex = new Regex(@"\b([A-F0-9]{64})\b", RegexOptions.Compiled);

            await Task.Run(() =>
            {
                foreach (string filePath in files)
                {
                    try
                    {
                        FileReader reader = new FileReader(filePath, new ScanOptions(), false, false, false, true);
                        FileResult result = reader.GetResult();

                        if (md5Regex.IsMatch(result.Content))
                        {
                            MatchCollection matches = md5Regex.Matches(result.Content);
                            for (int i = 0; i < matches.Count; i++)
                            {
                                md5Matches.Add(matches[i].Value);
                            }
                        }

                        if (sha1Regex.IsMatch(result.Content))
                        {
                            MatchCollection matches = sha1Regex.Matches(result.Content);
                            for (int i = 0; i < matches.Count; i++)
                            {
                                sha1Matches.Add(matches[i].Value);
                            }
                        }

                        if (sha256Regex.IsMatch(result.Content))
                        {
                            MatchCollection matches = sha256Regex.Matches(result.Content);
                            for (int i = 0; i < matches.Count; i++)
                            {
                                sha256Matches.Add(matches[i].Value);
                            }
                        }
                    }
                    catch (Exception) { }
                }
            });

            foreach (string md5 in md5Matches)
            {
                MD5s.Add(new HashInfo() { Hash = md5, HashType = HashType.MD5 });
            }

            foreach (string sha1 in sha1Matches)
            {
                SHA1s.Add(new HashInfo() { Hash = sha1, HashType = HashType.SHA1 });
            }

            foreach (string sha256 in sha256Matches)
            {
                SHA256s.Add(new HashInfo() { Hash = sha256, HashType = HashType.SHA256 });
            }

            MarkFree();
        }
    }
}
