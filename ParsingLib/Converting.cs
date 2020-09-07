using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib
{
    public class Converting
    {


        public List<string> ConvertSyntax(List<string> LineToConvert)
        {

            for (int i = 0; i < LineToConvert.Count(); i++)
            {
                LineToConvert[i] = LineToConvert[i].Replace("(*", "//");
                if (LineToConvert[i].Contains("boo"))
                {
                    LineToConvert[i] = LineToConvert[i].Replace(".", "AND");
                    LineToConvert[i] = LineToConvert[i].Replace("+", "OR");
                    LineToConvert[i] = LineToConvert[i].Replace("=", ":=");
                    LineToConvert[i] = LineToConvert[i].Replace("/", "NOT ");
                    LineToConvert[i] = LineToConvert[i].Replace("boo", "");



                }



            }
            return LineToConvert;
        }


    }

}
