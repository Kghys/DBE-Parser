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
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using System.Windows.Forms.VisualStyles;

namespace DBE_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog fileOpenPaths;
        FolderBrowserDialog fileSavePaths;
        List<string> fileLines = new List<string>();
        List<string> newFileLines = new List<string>();

        Analyze Analyzing = new Analyze();
        Converting converter = new Converting();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            fileOpenPaths = new OpenFileDialog();
            fileOpenPaths.Multiselect = true;

            fileOpenPaths.ShowDialog();

        }

        private void BtnConvertFile_Click(object sender, RoutedEventArgs e)
        {
            fileSavePaths = new FolderBrowserDialog();
            DialogResult result = fileSavePaths.ShowDialog();

            Console.WriteLine(fileSavePaths.SelectedPath);

            foreach (string fileName in fileOpenPaths.FileNames)
            {
                string onlyFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                newFileLines.Clear();
                fileLines.Clear();
                string line;
                StreamReader fileReader = new StreamReader(fileName);
                while ((line = fileReader.ReadLine()) != null)
                {
                    fileLines.Add(line);
                }
                fileReader.Close();

                newFileLines = converter.ConvertSyntax(fileLines);

                File.WriteAllLines(fileSavePaths.SelectedPath + "/" + onlyFileName + ".scl", newFileLines);
            }




            lstContent.ItemsSource = newFileLines;
            lstContent.Items.Refresh();


        }

        private void BtnAnalyzeFile_Click(object sender, RoutedEventArgs e)
        {
            int booCount = 0;
            int logCount = 0;
            int trfCount = 0;
            int tbwCount = 0;
            int finValCount = 0;
            int traCount = 0;
            int siCount = 0;
            int siNonCount = 0;
            int calCount = 0;
            int commentCount = 0;

            txtEditor.Clear();

            foreach (string fileName in fileOpenPaths.FileNames)
            {
                fileLines.Clear();
                int counter = 0;
                string line;
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

                booCount += Analyzing.booCount;
                trfCount += Analyzing.trfCount;
                logCount += Analyzing.logCount;
                tbwCount += Analyzing.tbwCount;
                finValCount += Analyzing.finValCount;
                traCount += Analyzing.traCount;
                siCount += Analyzing.siCount;
                siNonCount += Analyzing.siNonCount;
                calCount += Analyzing.calCount;
                commentCount += Analyzing.commentCount;
            }
            txtEditor.Text += $"\n Totale BOO's :  {booCount}";
            txtEditor.Text += $"\n Totale TRF's :  {trfCount}";
            txtEditor.Text += $"\n Totale LOG's :  {logCount}";
            txtEditor.Text += $"\n Totale TBW's :  {tbwCount}";
            txtEditor.Text += $"\n Totale FINVAL's :  {finValCount}";
            txtEditor.Text += $"\n Totale TRA's :  {traCount}";
            txtEditor.Text += $"\n Totale SI's :  {siCount}";
            txtEditor.Text += $"\n Totale SINON's :  {siNonCount}";
            txtEditor.Text += $"\n Totale CAL's :  {calCount}";
            txtEditor.Text += $"\n Totale Comments's :  {commentCount}";
        }



    }
}
