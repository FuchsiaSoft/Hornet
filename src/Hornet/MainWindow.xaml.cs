using Hornet.DatabaseManagement;
using Hornet.ViewModel.ViewModel;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hornet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            About aboutWindow = new About();
            aboutWindow.ShowDialog();
        }

        private void dockHash_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                ((MainViewModel)DataContext).HandleDroppedHashsetFiles(files);
            }
        }

        private void dockRegex_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                ((MainViewModel)DataContext).HandleDroppedRegexsetFiles(files);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //check to make sure that the root dir provided by the user
            //is accessible when impersonating as the provided credentials

            try
            {
                using (Impersonation.LogonUser(txtDomain.Text, txtUser.Text, pwd.SecurePassword, LogonType.Interactive))
                {
                    DirectoryInfo dir = new DirectoryInfo(txtRoot.Text);
                    dir.EnumerateDirectories();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Could not read directory, check location and/or credentials", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            ((MainViewModel)DataContext).StartScan(pwd.SecurePassword);
        }
    }
}
