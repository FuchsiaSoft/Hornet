using Hornet.ViewModel.ViewModel.DatabaseManagement;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Hornet.DatabaseManagement
{
    /// <summary>
    /// Interaction logic for AddEditHashSet.xaml
    /// </summary>
    public partial class AddEditHashSet : Window
    {
        public AddEditHashSet()
        {
            InitializeComponent();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                ((AddEditHashSetViewModel)DataContext).HandleDroppedFiles(files);
            }
        }
    }
}
