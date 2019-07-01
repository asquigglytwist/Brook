using Brook.MainWin.ViewModel;
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
        }
    }
}
