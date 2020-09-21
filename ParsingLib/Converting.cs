using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib
{
    public class Converting
    {


        public List<string> ConvertSyntax(List<string> LineToConvert,string blockName)
        {

            LineToConvert[0] = $"FUNCTION \"{blockName}\" : Void";
            LineToConvert[1] = "{ S7_Optimized_Acces := 'TRUE'}";
            LineToConvert[2] = "VERSION : 0.1";
            LineToConvert[3] = "BEGIN";


            for (int i = 0; i < LineToConvert.Count(); i++)
            {
                LineToConvert[i] = LineToConvert[i].Replace("(*", "//");
                if (LineToConvert[i].Contains("boo"))
                {
                    LineToConvert[i] = LineToConvert[i].Replace(".", "AND");
                    LineToConvert[i] = LineToConvert[i].Replace("+", "OR");
                    LineToConvert[i] = LineToConvert[i].Replace("/", "NOT ");
                    LineToConvert[i] = LineToConvert[i].Replace("boo", "");

                    if (LineToConvert[i].Contains("="))
                    {
                        string beforeEqual;
                        string afterEqual;
                        string[] splitString = LineToConvert[i].Split('=');

                        beforeEqual = splitString[0];
                        afterEqual = splitString[1];

                        LineToConvert[i] = afterEqual + " := " + beforeEqual;
                    }


                LineToConvert[i] = LineToConvert[i] + ";";
                }



            }

            LineToConvert.Add("END_FUNCTION");
            return LineToConvert;
        }


    }

}
