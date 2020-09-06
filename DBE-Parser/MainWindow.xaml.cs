﻿using Microsoft.Win32;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            file = new OpenFileDialog();
            file.Multiselect = true;

            file.ShowDialog();
            File.Copy(file.FileName, @"C:\Users\DBE-KG\Documents\GitHub\DBE-Parser\DBE-Parser\Res\1.txt", true);

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
            fileLines.Clear();
            int counter = 0;
            string line;
            StreamReader fileReader = new StreamReader(@"C:\Users\DBE-KG\Documents\GitHub\DBE-Parser\DBE-Parser\Res\1.txt");

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
        }

        
    }
}
