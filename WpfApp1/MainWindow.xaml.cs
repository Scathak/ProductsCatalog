using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Text;
using System.Data;
using System.Windows.Markup;
using System.Xml;

namespace WpfApp1 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProductContext _context = new ProductContext();

        private CollectionViewSource categoryViewSource;

        public MainWindow()
        {
            InitializeComponent();
            categoryViewSource = (CollectionViewSource)FindResource(nameof(categoryViewSource));
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // this is for demo purposes only, to make it easier
            // to get up and running
            _context.Database.EnsureCreated();
            // load the entities into EF Core
            _context.Categories.Load();
            // bind to the source
            categoryViewSource.Source =
                _context.Categories.Local.ToObservableCollection();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var range = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
            var fStream = new FileStream("richtextbox.xaml", FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            fStream.Close();
            var ProductOnFocus = productsDataGrid.SelectedItem as Product;
            if (ProductOnFocus != null)
            { 
                var ms = new MemoryStream();
                range.Save(ms, DataFormats.XamlPackage);
                ProductOnFocus.Description = ms.ToArray();
            }
            // all changes are automatically tracked, including
            // deletes!
            _context.SaveChanges();
            // this forces the grid to refresh to latest values
            categoryDataGrid.Items.Refresh();
            productsDataGrid.Items.Refresh();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // clean up database connections
            _context.Dispose();
            base.OnClosing(e);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var _fileName = openFileDialog.FileName;
                if (File.Exists(_fileName))
                {
                    var range = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                    var fStream = new FileStream(_fileName, FileMode.OpenOrCreate);
                    range.Load(fStream, DataFormats.XamlPackage);
                    fStream.Close();
                }
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var  pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {
                //use either one of the below
                pd.PrintVisual(mainRTB as Visual, "printing as visual");
                //pd.PrintDocument((((IDocumentPaginatorSource)mainRTB.Document).DocumentPaginator), "printing as paginator");
            }
        }
        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ProductOnFocus =  productsDataGrid.SelectedItem as Product;
            if (ProductOnFocus != null) 
            {
                var ContentRange = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                var ms = new MemoryStream();
                ms.Write(ProductOnFocus.Description, 0, ProductOnFocus.Description.Length);
                ContentRange.Load(ms, DataFormats.XamlPackage);
            }
        }
    }
}