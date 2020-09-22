using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib
{
    public class Converting
    {


        public List<string> ConvertSyntax(List<string> LineToConvert, string blockName)
        {
            string[] declaration = { "VERSION : 0.1", "BEGIN" };
            LineToConvert[0] = $"FUNCTION \"{blockName}\" : Void";
            LineToConvert[1] = "{ S7_Optimized_Acces := 'TRUE'}";
            LineToConvert.InsertRange(2, declaration);


            for (int i = 0; i < LineToConvert.Count(); i++)
            {
                ConvertSigns(LineToConvert, i);

                if (LineToConvert[i].Contains(" gs "))
                {
                    try
                    {
                        if (LineToConvert[i].Contains("boo"))
                        {
                            LineToConvert[i] = LineToConvert[i].Replace("boo ", "");
                        }
                        List<string> toBeInserted = GsHandle(LineToConvert, i);
                        LineToConvert.InsertRange(i, toBeInserted);
                        LineToConvert.RemoveAt(i + toBeInserted.Count());
                    }
                    catch (Exception)
                    {

                        throw;
                    }


                }

                if (LineToConvert[i].Contains("= val"))
                {

                    LineToConvert[i] = LineToConvert[i].Replace("boo", "IF(");
                    LineToConvert[i] = LineToConvert[i].Replace("= val", ") THEN");

                }
                if (LineToConvert[i].Contains("finval"))
                {
                    LineToConvert[i] = LineToConvert[i].Replace("finval", "END_IF;");
                }
                if (LineToConvert[i].Contains("boo"))
                {
                    SwitchSidesAndDeleteOperand(LineToConvert, i, "boo");

                    if (!LineToConvert[i].Contains("THEN")) LineToConvert[i] = LineToConvert[i] + ";";
                }
                if (LineToConvert[i].Contains("log"))
                {
                    SwitchSidesAndDeleteOperand(LineToConvert, i, "log");

                    if (!LineToConvert[i].Contains("THEN")) LineToConvert[i] = LineToConvert[i] + ";";
                }
                if (LineToConvert[i].Contains("cal"))
                {
                    SwitchSidesAndDeleteOperand(LineToConvert, i, "cal");

                    if (!LineToConvert[i].Contains("THEN")) LineToConvert[i] = LineToConvert[i] + ";";
                }

            }

            LineToConvert.Add("END_FUNCTION");
            return LineToConvert;
        }

        private static void SwitchSidesAndDeleteOperand(List<string> LineToConvert, int i,string operand)
        {
            LineToConvert[i] = LineToConvert[i].Replace($"{operand} ", "");
            if (LineToConvert[i].Contains(" = "))
            {
                string beforeEqual;
                string afterEqual;
                string[] splitString = LineToConvert[i].Split('=');

                beforeEqual = splitString[0];
                afterEqual = splitString[1];

                LineToConvert[i] = afterEqual + " := " + beforeEqual;
            }
        }

        private static List<string> GsHandle(List<string> LineToConvert, int i)
        {
            List<string> insertingGs = new List<string>();
            LineToConvert[i] = LineToConvert[i].Replace(" = gs", " @  gs");
            string[] equalString = LineToConvert[i].Split('@');
            string[] commaSplit = equalString[1].Split(',');
            commaSplit[0] = commaSplit[0].Replace(" gs ", "");

            insertingGs.Add($"IF({equalString[0]} AND {commaSplit[0]} ) THEN");
            insertingGs.Add($"{commaSplit[0]} := 0");
            insertingGs.Add($"{commaSplit[1]} := 1");
            insertingGs.Add($"END_IF;");


            return insertingGs;
        }

        private static void ConvertSigns(List<string> LineToConvert, int i)
        {
            LineToConvert[i] = LineToConvert[i].Replace("(* ", " //");
            LineToConvert[i] = LineToConvert[i].Replace(" OX ", " XOR ");
            LineToConvert[i] = LineToConvert[i].Replace(" . ", " AND ");
            LineToConvert[i] = LineToConvert[i].Replace(" + ", " OR ");
            LineToConvert[i] = LineToConvert[i].Replace(" / ", " NOT ");
            LineToConvert[i] = LineToConvert[i].Replace("FIN_BLOC_FONCTION", "");

        }
    }

}
