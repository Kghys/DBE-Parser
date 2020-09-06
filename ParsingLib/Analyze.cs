﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib
{
    public class Analyze
    {

        public void CountBoos(List<string> LineToAnalyze)
        {
            int booCount = 0;
            int logCount = 0;
            int trfCount = 0;
            int commentCount = 0;
            for (int i = 0; i < LineToAnalyze.Count(); i++)
            {
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
            }
            Console.WriteLine($"er waren {booCount} BOO's");
            Console.WriteLine($"er waren {logCount} LOG's");
            Console.WriteLine($"er waren {trfCount} TRF's");
            Console.WriteLine($"er waren {commentCount} comments");
        }

    }
}
