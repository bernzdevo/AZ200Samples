using RetryPattern.AppServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace RetryPattern
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StorageService storageService = new StorageService();

        public MainWindow()
        {
            InitializeComponent();
       
        }

        public string CheckConnection()
        {
           tblkResult.Text = "";
            string result =  storageService.ReadWriteToStorage();
            Console.WriteLine(result);
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tblkResult.Text= CheckConnection();
        }
    }
}
