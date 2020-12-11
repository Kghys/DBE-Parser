using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParsingLib
{
    public class Converting
    {


        public List<string> ConvertSyntax(List<string> Lines, string blockName)
        {




            
            var LinesToConvert = MasterRelayHandler(Lines);

            for (int i = 3; i < LinesToConvert.Count(); i++)
            {
                ConvertSigns(LinesToConvert, i);
                LinesToConvert[i] = AddVariables(LinesToConvert[i]);


                if (LinesToConvert[i].Contains(" gs "))
                {
                    try
                    {
                        if (LinesToConvert[i].Contains("boo"))
                        {
                            LinesToConvert[i] = LinesToConvert[i].Replace("boo ", "");
                        }
                        List<string> toBeInserted = GsHandle(LinesToConvert, i);
                        LinesToConvert.InsertRange(i, toBeInserted);
                        LinesToConvert.RemoveAt(i + toBeInserted.Count());
                    }
                    catch (Exception)
                    {

                        throw;
                    }


                }

                if (LinesToConvert[i].Contains("= val"))
                {

                    LinesToConvert[i] = LinesToConvert[i].Replace("boo", "IF(");
                    LinesToConvert[i] = LinesToConvert[i].Replace("= val", ") THEN");

                }
                if (LinesToConvert[i].Contains("finval"))
                {
                    LinesToConvert[i] = LinesToConvert[i].Replace("finval", "END_IF;");
                }
                if (LinesToConvert[i].Contains("boo"))
                {
                    SwitchSidesAndDeleteOperand(LinesToConvert, i, "boo");

                    if (!LinesToConvert[i].Contains("THEN")) LinesToConvert[i] = LinesToConvert[i] + ";";
                }


                if (LinesToConvert[i].Contains("log"))
                {
                    SwitchSidesAndDeleteOperand(LinesToConvert, i, "log");

                    if (!LinesToConvert[i].Contains("THEN")) LinesToConvert[i] = LinesToConvert[i] + ";";
                }
                if (LinesToConvert[i].Contains("cal"))
                {
                    SwitchSidesAndDeleteOperand(LinesToConvert, i, "cal");

                    if (!LinesToConvert[i].Contains("THEN")) LinesToConvert[i] = LinesToConvert[i] + ";";
                }
                // SI ALORS SINON
                i = HandleIFs(LinesToConvert, i);
            }

            LinesToConvert.Add("END_FUNCTION");
            BlockDeclaration(LinesToConvert, blockName);

            return LinesToConvert;


        }

        private static List<string> MasterRelayHandler(List<string> ProgramLines)
        {
            int startLocation = 0;
            int stopLocation = 0;
            List<string> returnList;
            foreach (var line in ProgramLines.ToList())
            {
                //Begin van de MR lus
                if (line.Contains("=") && line.Contains("mr"))
                {
                    startLocation = ProgramLines.IndexOf(line);

                    ProgramLines[startLocation] = line.Replace(" = mr", " @  mr");
                    string[] equalString = ProgramLines[startLocation].Split('@');
                    ProgramLines[startLocation] = ($"IF({equalString[0]}) THEN (*START OF MasterRelay");
                    List<string> tagsToBeReset = new List<string>();

                    for (int i = startLocation; i < ProgramLines.Count(); i++)
                    {
                        if (ProgramLines[i].Contains("gs"))
                        {
                            if (ProgramLines[i].Contains("boo"))
                            {
                                ProgramLines[i] = ProgramLines[i].Replace("boo ", "");
                            }

                            List<string> toBeInserted = GsHandle(ProgramLines, i);
                            ProgramLines.InsertRange(i, toBeInserted);
                            ProgramLines.RemoveAt(i + toBeInserted.Count());

                            foreach (string insertedLine in toBeInserted)
                            {
                                if (insertedLine.Contains(":="))
                                {
                                    insertedLine.Replace(":=", ":");
                                    string[] splitString = insertedLine.Split(':');
                                    tagsToBeReset.Add($"{splitString[0].Trim()} := 0");
                                }
                            }


                            i += toBeInserted.Count() -1; 
                        }
                        if (ProgramLines[i].Contains("finmr"))
                        {
                            stopLocation = i;
                            ProgramLines[stopLocation] = ($"ELSE");
                            ProgramLines.Insert(stopLocation + 1,($"(*ADD GS_CODE_RESETTING"));
                            ProgramLines.InsertRange(stopLocation + 2, tagsToBeReset);
                            ProgramLines.Insert(stopLocation + 2 + tagsToBeReset.Count(), ($"END_IF; (*END OF MasterRelay"));
                            break;
                        }
                    }

                }
            }
            returnList = ProgramLines;
            return returnList;

        }

        private static int HandleIFs(List<string> LineToConvert, int i)
        {
            var index = i;
            if (LineToConvert[i].Contains("alors"))
            {

                LineToConvert[i - 1] = LineToConvert[i - 1] + LineToConvert[i].Replace("alors", " THEN");
                LineToConvert.RemoveAt(i);
                index -= 1;
            }
            if (LineToConvert[i].Contains("sinon"))
            {

                LineToConvert[i] = LineToConvert[i].Replace("sinon", "ELSE");

            }
            if (LineToConvert[i].Contains("finsi"))
            {

                LineToConvert[i] = LineToConvert[i].Replace("finsi", "END_IF;");

            }
            if (LineToConvert[i].Contains("si"))
            {

                LineToConvert[i] = LineToConvert[i].Replace("si", "IF");
                LineToConvert[i] = LineToConvert[i].Replace("{", "(");
                LineToConvert[i] = LineToConvert[i].Replace("}", ")");

            }

            return index;
        }

        private static string AddVariables(string LineToConvert)
        {
            string[] spaceSplit = LineToConvert.Split(' ');

            for (int j = 0; j < spaceSplit.Length; j++)
            {
                if (spaceSplit[j].StartsWith("//"))
                {
                    break;
                }
                if (spaceSplit[j].StartsWith("EC"))
                {
                    spaceSplit[j] = $"{spaceSplit[j]}();";
                }
                else if (spaceSplit[j].Length == 5 && Regex.Match(spaceSplit[j], @"\b([A-Z])([0-F])([0-F])([0-F])([0-F])").Success)
                {
                    spaceSplit[j] = $"\"{spaceSplit[j]}\"";
                }


            }
            return string.Join(" ", spaceSplit);
        }

        private static void BlockDeclaration(List<string> LineToConvert, string blockName)
        {
            string[] declaration = { "VERSION : 0.1", "BEGIN" };
            LineToConvert[0] = $"FUNCTION \"{blockName}\" : Void";
            LineToConvert[1] = "{ S7_Optimized_Acces := 'TRUE'}";
            LineToConvert.InsertRange(2, declaration);
        }

        private static void SwitchSidesAndDeleteOperand(List<string> LineToConvert, int i, string operand)
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
            //GS = voorwaarden voldaan + eerste bool na de GS is 1 => tweede bool op 1 en eerste bool resetten.
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
            LineToConvert[i] = LineToConvert[i].Replace("/", " NOT ");
            LineToConvert[i] = LineToConvert[i].Replace("(* ", " //");
            LineToConvert[i] = LineToConvert[i].Replace("(*", "//");
            LineToConvert[i] = LineToConvert[i].Replace(" OX ", " XOR ");
            LineToConvert[i] = LineToConvert[i].Replace(" . ", " AND ");
            LineToConvert[i] = LineToConvert[i].Replace(" + ", " OR ");
            LineToConvert[i] = LineToConvert[i].Replace("FIN_BLOC_FONCTION", "");

        }
    }

}
