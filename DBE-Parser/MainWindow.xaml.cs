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
using System.Linq;

namespace DBE_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog fileOpenPaths;
        FolderBrowserDialog fileSavePaths;
        private List<string> fileLines = new List<string>();
        private List<string> giantFile = new List<string>();
        private List<string> newFileLines = new List<string>();
        private List<string> tagsInProgram = new List<string>();
        private List<string> operandsInProgram = new List<string>();
        private List<int> operandsCounted = new List<int>();

        Analyze Analyzing = new Analyze();
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
            try
            {
                fileSavePaths = new FolderBrowserDialog();
                fileSavePaths.SelectedPath = Path.GetDirectoryName(fileOpenPaths.FileNames[0].ToString());
                DialogResult result = fileSavePaths.ShowDialog();
                var progressValue = 0;
                txtConverted.Clear();


                tagHelper.MakeTags();
                Analyzing = new Analyze();

                Console.WriteLine(fileSavePaths.SelectedPath);
                progress.Maximum = fileOpenPaths.FileNames.Length;
                foreach (string fileName in fileOpenPaths.FileNames)
                {
                    //progressbar
                    progressValue++;
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
                    newFileLines = testConverting.Convert(fileLines, onlyFileName, tagHelper) ;

                    //analyze converted syntaxes
                    operandsCounted = Analyzing.CountBoos(newFileLines, operandsInProgram);

                    File.WriteAllLines(fileSavePaths.SelectedPath + "/" + onlyFileName + ".scl", newFileLines);
                    progress.Value = progressValue;
                }

                File.WriteAllLines(fileSavePaths.SelectedPath + "/BigProgram-Source.scl", giantFile);
                // alles in 1 file gieten
                WriteExcel(fileSavePaths.SelectedPath);

                for (int i = 0; i < operandsInProgram.Count(); i++)
                {

                    txtConverted.AppendText($"{operandsInProgram[i]} count : {operandsCounted[i]}\n");

                }

                
            }
            catch (Exception Ex)
            {

                System.Windows.MessageBox.Show(Ex.Message);
            }


        }

        private void WriteExcel(string path)
        {
            //Build excel file
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
                FileInfo excelFile = new FileInfo(path + "/TagTable.xlsx");
                excel.SaveAs(excelFile);
            }


        }

        private void BtnAnalyzeFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int totalLines = 0;
                //grote file clearen
                giantFile.Clear();
                txtEditor.Clear();
                operandsInProgram = new List<string>();
                tagHelper = new TagHelper();
                Analyzing = new Analyze();

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

                    tagsInProgram = tagHelper.ConvertVariables(fileLines);
                    operandsInProgram = tagHelper.Operands;

                    operandsCounted = Analyzing.CountBoos(fileLines, operandsInProgram);

                }

                for (int i = 0; i < operandsInProgram.Count(); i++)
                {

                    txtEditor.AppendText($"{operandsInProgram[i]} count : {operandsCounted[i]}\n");

                }
            }
            catch (Exception Ex)
            {

                System.Windows.MessageBox.Show(Ex.Message);
            }

        }



    }
}
