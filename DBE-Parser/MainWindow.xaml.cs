using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ParsingLib;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using CsvHelper;
using OfficeOpenXml;
using ParsingLib.Services;

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
        List<string> giantFile = new List<string>();
        List<string> newFileLines = new List<string>();
        List<string> tagsInProgram = new List<string>();

        Analyze Analyzing = new Analyze();
        Converting converter = new Converting();
        //in testing
        Converter testConverting = new Converter();
        TagHelper tagHelper = new TagHelper();

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
            fileSavePaths.SelectedPath = Path.GetDirectoryName(fileOpenPaths.FileNames[0].ToString());
            DialogResult result = fileSavePaths.ShowDialog();
            txtConverted.Clear();
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
            int totalLines = 0;

            Console.WriteLine(fileSavePaths.SelectedPath);
            try
            {
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
                    newFileLines = testConverting.Convert(fileLines,onlyFileName);

                    //analyze converted syntaxes
                    Analyzing.CountBoos(newFileLines);
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

                    File.WriteAllLines(fileSavePaths.SelectedPath + "/" + onlyFileName + ".scl", newFileLines);
                }

                File.WriteAllLines(fileSavePaths.SelectedPath + "/BigProgram.scl", giantFile);
                // alles in 1 file gieten
                WriteExcel(fileSavePaths.SelectedPath);


                txtConverted.Text += $"\n Totale BOO's :  {booCount}";
                txtConverted.Text += $"\n Totale TRF's :  {trfCount}";
                txtConverted.Text += $"\n Totale LOG's :  {logCount}";
                txtConverted.Text += $"\n Totale TBW's :  {tbwCount}";
                txtConverted.Text += $"\n Totale FINVAL's :  {finValCount}";
                txtConverted.Text += $"\n Totale TRA's :  {traCount}";
                txtConverted.Text += $"\n Totale SI's :  {siCount}";
                txtConverted.Text += $"\n Totale SINON's :  {siNonCount}";
                txtConverted.Text += $"\n Totale CAL's :  {calCount}";
                txtConverted.Text += $"\n Totale Comments's :  {commentCount}";
                txtConverted.Text += $"\n Totale Lijnen :  {totalLines}";

            }
            catch (Exception)
            {

                return;
            }


        }

        private void WriteExcel(string path)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("PLC Tags");

                var headerRow = new List<string[]>()
                            {
                                new string[] { "Name", "Path", "Data Type", "Logical Address", "Comment", "Hmi Visible", "Hmi Accessible", "Hmi Writeable", "Typeobject ID", "Version ID" }
                            };

                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                var worksheet = excel.Workbook.Worksheets["PLC Tags"];

                worksheet.Cells[headerRange].LoadFromArrays(headerRow);


                worksheet.Cells[2, 1].LoadFromCollection(tagHelper.TagList);
                FileInfo excelFile = new FileInfo(path + "/Test.xlsx");
                excel.SaveAs(excelFile);
            }


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
            int totalLines = 0;
            //grote file clearen
            giantFile.Clear();
            txtEditor.Clear();

            foreach (string fileName in fileOpenPaths.FileNames)
            {


                fileLines.Clear();
                int counter = 0;
                string line;
                StreamReader fileReader = new StreamReader(fileName);
                while ((line = fileReader.ReadLine()) != null)
                {
                    counter++;
                    fileLines.Add(line);
                    //de lijn ook in de grote file steken.
                    giantFile.Add(line);
                    totalLines += 1;

                }

                fileReader.Close();
                Console.WriteLine("There were {0} lines.", counter);

                Analyzing.CountBoos(fileLines);
                tagsInProgram = tagHelper.ConvertVariables(fileLines);

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
            txtEditor.Text += $"\n Totale Lijnen :  {totalLines}";
        }



    }
}
