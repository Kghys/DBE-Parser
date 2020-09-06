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
using ParsingLib;

namespace DBE_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog file;
        List<string> fileLines = new List<string>();
        Analyze Analyzing = new Analyze();

        int booCount = 0;
        int trfCount = 0; 
        int logCount = 0;
        int commentCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            file = new OpenFileDialog();
            file.Multiselect = true;

            file.ShowDialog();

            foreach (String fileName in file.FileNames)
            {
                txtEditor.Text += $"\n  {fileName }";
            }

        }

        private void BtnExitFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConvertFile_Click(object sender, RoutedEventArgs e)
        {
            booCount = 0;
            foreach (string fileName in file.FileNames)
            {
                fileLines.Clear();
                int counter = 0;
                string line;
                //StreamReader fileReader = new StreamReader(@"C:\Users\DBE-KG\Documents\GitHub\DBE-Parser\DBE-Parser\Res\1.txt");
                StreamReader fileReader = new StreamReader(fileName);
                while ((line = fileReader.ReadLine()) != null)
                {
                    System.Console.WriteLine(line);
                    counter++;
                    fileLines.Add(line);

                }

                fileReader.Close();
                Console.WriteLine("There were {0} lines.", counter);
                lstContent.ItemsSource = fileLines;
                lstContent.Items.Refresh();

                Analyzing.CountBoos(fileLines);

                booCount +=  Analyzing.booCount;
                trfCount +=  Analyzing.trfCount;
                logCount +=  Analyzing.logCount;
                commentCount +=  Analyzing.commentCount;
            }
            txtEditor.Text += $"\n Totale BOO's :  {booCount}";
            txtEditor.Text += $"\n Totale TRF's :  {trfCount}";
            txtEditor.Text += $"\n Totale LOG's :  {logCount}";
            txtEditor.Text += $"\n Totale Comments's :  {commentCount}";
        }

        

    }
}
