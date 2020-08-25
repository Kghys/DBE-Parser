using Microsoft.Win32;
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

namespace DBE_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;

            openFileDialog.ShowDialog();
                

            foreach (String fileName in openFileDialog.FileNames)
            {
                txtEditor.Text += $"\n  {fileName }";
            }
            
        }

        private void btnExitFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnConvertFile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
