using System.Windows;
using System.Windows.Controls;
using CsvImport.Database;
using CsvImport.Wpf.Mvvm;

namespace CsvImport.Wpf
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var dbManager = new DbManager();
            _viewModel = new MainViewModel(dbManager);
            DataContext = _viewModel;
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.Connect();
        }

        private async void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await _viewModel.StartEditItem();
        }
    }
}