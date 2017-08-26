using GalaSoft.MvvmLight.CommandWpf;
using Hornet.IO.TextParsing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hornet.ViewModel.ViewModel.DatabaseManagement
{
    public class AddEditRegexSetViewModel : WindowCreatingViewModel
    {

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

        private ObservableCollection<RegexInfo> _RegexInfos = new ObservableCollection<RegexInfo>();

        public ObservableCollection<RegexInfo> RegexInfos
        {
            get { return _RegexInfos; }
            set
            {
                _RegexInfos = value;
                RaisePropertyChanged("RegexInfos");
            }
        }


        public AddEditRegexSetViewModel()
        {
            WindowTitle = "Create new regular expression set definition";
        }

        public AddEditRegexSetViewModel(RegexInfoGroup group)
        {
            WindowTitle = "Edit regular expression set definition";
            Name = group.Name;
            Description = group.Description;
            RegexInfos = new ObservableCollection<RegexInfo>(group.RegexInfos);
        }

        public ICommand CancelCommand { get { return new RelayCommand(CloseWindow); } }

        public ICommand SaveCommand { get { return new RelayCommand(Save); } }

        private async void Save()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Choose location to save regex set file";
            dlg.OverwritePrompt = true;
            dlg.AddExtension = true;
            dlg.DefaultExt = ".rset";
            dlg.Filter = "Regex set definition (*.rset)|*.rset";
            if (dlg.ShowDialog() == true)
            {
                MarkBusy("Saving file...");
                await Task.Run(() =>
                {
                    try
                    {
                        RegexInfoGroup group = new RegexInfoGroup();
                        if (string.IsNullOrWhiteSpace(Name))
                        {
                            group.Name = "Un-named regular expression set";
                        }
                        else
                        {
                            group.Name = Name;
                        }

                        group.Description = Description ?? string.Empty;

                        group.RegexInfos.AddRange(RegexInfos);

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
    }
}
