using Brook.MainWin.View;
using Brook.MainWin.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Brook.MainWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DetailsViewModel detailsVM;
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        public MainWindow()
        {
            InitializeComponent();
            InitDVM(@"C:\TestData\");
        }

        private void InitDVM(string rootDir)
        {
            detailsVM = new DetailsViewModel(rootDir);
            DataContext = detailsVM;
            lsvSoundFiles.ItemsSource = detailsVM.AllSoundFiles;
            // [BIB]:  https://www.wpf-tutorial.com/listview-control/listview-grouping/
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lsvSoundFiles.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("AlbumArtHash");
            view.GroupDescriptions.Add(groupDescription);
            view.Filter = UserFilter;
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                return true;
            }
            else
            {
                return ((item as Model.SoundFile).Album.IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void lsvSoundFilesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var column = (sender as GridViewColumnHeader);
            var sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lsvSoundFiles.Items.SortDescriptions.Clear();
            }
            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
            {
                newDir = ListSortDirection.Descending;
            }
            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lsvSoundFiles.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lsvSoundFiles.ItemsSource).Refresh();
        }
    }
}
