﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib
{
    public class Analyze
    {

        public int booCount { get; set; }
        public int logCount { get; set; }
        public int trfCount { get; set; }
        public int tbwCount { get; set; }
        public int finValCount { get; set; }
        public int commentCount { get; set; }
        public int traCount { get; set; }
        public int siCount { get; set; }
        public int siNonCount { get; set; }
        public int calCount { get; set; }



        public void CountBoos(List<string> LineToAnalyze)
        {
            booCount = 0;
            logCount = 0;
            trfCount = 0;
            tbwCount = 0;
            finValCount = 0;
            traCount = 0;
            siCount = 0;
            siNonCount = 0;
            calCount = 0;
            commentCount = 0;
            for (int i = 0; i < LineToAnalyze.Count(); i++)
            {
                if (LineToAnalyze[i].Contains("si") && LineToAnalyze[i+1].Contains("alors"))
                {
                    siCount++;
                }
                if (LineToAnalyze[i].Contains("sinon"))
                {
                    siNonCount++;
                }
                if (LineToAnalyze[i].Contains("boo"))
                {
                    booCount++;
                }
                if (LineToAnalyze[i].Contains("trf"))
                {
                    trfCount++;
                }
                if (LineToAnalyze[i].Contains("log"))
                {
                    logCount++;
                }
                if (LineToAnalyze[i].Contains("(*"))
                {
                    commentCount++;
                }
                if (LineToAnalyze[i].Contains("tbw"))
                {
                    tbwCount++;
                }
                if (LineToAnalyze[i].Contains("finval"))
                {
                    finValCount++;
                }
                if (LineToAnalyze[i].Contains("tra"))
                {
                    traCount++;
                }
                if (LineToAnalyze[i].Contains("cal"))
                {
                    calCount++;
                }
            }
            Console.WriteLine($"er waren {booCount} BOO's");
            Console.WriteLine($"er waren {logCount} LOG's");
            Console.WriteLine($"er waren {trfCount} TRF's");
            Console.WriteLine($"er waren {commentCount} comments");
        }

    }
}
