using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParsingLib.Entities;

namespace ParsingLib.Services
{
    public class Converter
    {

        List<string> ConvertedProgram;
        List<string> ProgramToConvert;
        private List<Tag> tags = new List<Tag>();
        private string ldx = "";
        private string stx = "";

        public List<string> Convert(List<string> Program, string BlockName, TagHelper tagHelper)
        {
            tags = tagHelper.TagList;
            this.ProgramToConvert = Program;
            ConvertedProgram = new List<string>();
            //start with blockdeclaration
            BlockDeclaration(BlockName);

            for (int i = 0; i < ProgramToConvert.Count(); i++)
            {
                var line = ProgramToConvert[i];
                if (!(line.Trim() == ""))
                {
                    i = ConvertLine(line, i);
                }
                else
                {
                    ConvertedProgram.Add($"");
                }
            }
            ConvertedProgram.Add($"");
            for (int i = 0; i < ProgramToConvert.Count(); i++)
            {
                var line = ProgramToConvert[i];
                if (!(line.Trim() == ""))
                {
                    ConvertedProgram.Add($"// {line}");
                }
            }

            ConvertedProgram.Add("END_FUNCTION");

            return ConvertedProgram;
        }

        private void BlockDeclaration(string blockName)
        {
            ConvertedProgram.Add($"FUNCTION \"{blockName}\" : Void");
            ConvertedProgram.Add("{ S7_Optimized_Acces := 'TRUE'}");
            ConvertedProgram.Add("VERSION : 0.1");
            ConvertedProgram.Add("BEGIN");
            ConvertedProgram.Add($"");
        }


        private int ConvertLine(string programLine, int i)
        {
            var index = i;
            var newIndex = index;
            AddVariables(index);
            //ConvertedProgram.Add($"");
            //ConvertedProgram.Add($"// {ProgramToConvert[index]}");
            if (programLine.Contains("(*")) { ConvertedProgram.Add(ProgramToConvert[index].Replace("(*", "//")); return newIndex; }
            //REGEX: Kijk of er iets inzit dan lijkt op het volgende: TXXX_XXXX (zoals T16_W0104)
            if (Regex.IsMatch(programLine, @"(T[0-9]*[0-9]*[0-9]_[A-Z][0-9][0-9A-Z][0-9A-Z][0-9A-Z])")) { HandleTUnderscore(index); return newIndex; }
            if (programLine.Contains("= mr")) { newIndex = HandleMr(index); return newIndex; }
            if (programLine.Contains("gs ")) { HandleGS(index); return newIndex; }
            if (programLine.Contains("val")) { HandleVal(index); return newIndex; }
            if (programLine.Contains("tra ")) { HandleTra(index); return newIndex; }
            if (programLine.Contains("cal ")) { HandleCal(index); return newIndex; }
            if (programLine.Contains("log ")) { HandleLog(index); return newIndex; }
            if (programLine.Contains("si ")) { HandleSi(index); return newIndex; }
            if (programLine.Contains("tbw ")) { HandleTbw(index); return newIndex; }
            if (programLine.Contains("twb ")) { HandleTwb(index); return newIndex; }
            if (programLine.Contains("ded ")) { HandleDed(index); return newIndex; }
            if (programLine.Contains("deg ")) { HandleDeg(index); return newIndex; }
            if (programLine.Contains("cas_de ")) { HandleCas(index); return newIndex; }
            if (programLine.Contains("quand ")) { HandleQuand(index); return newIndex; }
            if (programLine.Contains("autres")) { HandleAutres(index); return newIndex; }
            if (programLine.Contains("fincas")) { HandleFincas(index); return newIndex; }
            if (programLine.Contains("stx ")) { HandleStx(index); return newIndex; }
            if (programLine.Contains("ldx ")) { HandleLdx(index); return newIndex; }
            if (programLine.Contains("(X)")) { HandleX(index); return newIndex; }
            if (programLine.Contains("fm ")) { HandleFm(index); return newIndex; }
            if (programLine.Contains("fd ")) { HandleFd(index); return newIndex; }
            if (programLine.Contains("jmp ")) { HandleJmp(index); return newIndex; }
            if (programLine.Contains("label ")) { HandleLabel(index); return newIndex; }
            if (programLine.Contains("alors")) { return newIndex; }
            if (programLine.Contains("sinon")) { HandleSinon(index); return newIndex; }
            if (programLine.Contains("finsi")) { HandleFinsi(index); return newIndex; }
            if (programLine.Contains("trf ")) { HandleTrf(index); return newIndex; }
            if (programLine.Contains(" set")) { HandleSet(index); return newIndex; }
            if (programLine.Contains(" reset")) { HandleReset(index); return newIndex; }
            if (programLine.Contains("boo ")) { HandleBoo(index); return newIndex; }
            if (programLine.Contains("FIN")) { return newIndex; }
            if (programLine.Contains("BLOC")) { return newIndex; }
            if (programLine.Contains("VAR")) { return newIndex; }
            if (programLine.Contains("PROGRAMME")) { return newIndex; }


            ConvertedProgram.Add($"{ProgramToConvert[index]}");
            ConvertedProgram.Add($"//No translation found");



            // default implementeren, unknown
            return newIndex;
        }

        private string GetAdress(string tagName)
        {
            tagName = tagName.Trim().Replace("\"","");
            string directAdress = "";

            if (tags.Where(t => t.Name == tagName).FirstOrDefault() != null)
            {
                directAdress = tags.Where(t => t.Name == tagName).FirstOrDefault().LogicalAddress.Replace("%MW", "").Replace("%I","").Replace("%Q","");
            }
            return directAdress;
        }

        private void HandleX(int index)
        {
            //get line
            var line = ProgramToConvert[index].Replace("trf ", ""); 
            line = line.Replace("(X)", "");
            var splitLine = line.Split('=');

            //Check if time
            if (line.Contains("\"F"))
            {
                string wordAddress = splitLine[1][3].ToString() + splitLine[1][4].ToString() + splitLine[1][5].ToString() + splitLine[1][6].ToString();
                
                var decValueWord = int.Parse(wordAddress, System.Globalization.NumberStyles.HexNumber);
                // "Timers".T[59].PT := "B0120";
                splitLine[1] = $"\"Timers\".T[{decValueWord}].PT";

                string directAdress = GetAdress(splitLine[0]);

                line = $"{splitLine[1]} := WORD_TO_TIME(PEEK_WORD(area:= 16#83, dbNumber := 0, byteOffset := {directAdress} + {ldx})*100);";
                
            }
            ConvertedProgram.Add(line);
        }
        private void HandleLdx(int index)
        {
            var replacedString = ProgramToConvert[index].Replace("ldx", "");
            ldx = replacedString;
        }

        private void HandleStx(int index)
        {
            var replacedString = ProgramToConvert[index].Replace("stx", "");
            stx = replacedString;
        }

        private void HandleTUnderscore(int index)
        {
            //  trf T16_XXXX = TXX_XXXX
            var replacedString = ProgramToConvert[index];
            string[] splitString = Regex.Split(replacedString, @"([ _T])");
            var theList = splitString.ToList();
            for (int i = 0; i < splitString.Count(); i++)
            {
                if (splitString[i].Equals("") || splitString[i].Contains(' '))
                {
                    theList.RemoveAt(theList.IndexOf(splitString[i]));
                }
            }

            var newLine = "";

            foreach (var item in theList)
            {
                if (!item.Contains("trf") && !item.Contains("T"))
                {
                    newLine += item.Trim() + " ";

                }
            }

            var equalString = newLine.Split('=');
            var spaceSplitBeforeEqual = equalString[0].Trim().Split(' ');
            var spaceSplitAfterEqual = equalString[1].Trim().Split(' ');

            if (Regex.IsMatch(newLine, @"([W][0-9][0-9A-Z][0-9A-Z][0-9A-Z])") && Regex.IsMatch(newLine, @"([A][0-9][0-9A-Z][0-9A-Z][0-9A-Z])")) // is het een woord dat naar een output schrijft?
            {
                string wordAddress = spaceSplitBeforeEqual[0][2].ToString() + spaceSplitBeforeEqual[0][3].ToString() + spaceSplitBeforeEqual[0][4].ToString() + spaceSplitBeforeEqual[0][5].ToString();
                string wordAddress2 = spaceSplitAfterEqual[2][1].ToString() + spaceSplitAfterEqual[2][2].ToString() + spaceSplitAfterEqual[2][3].ToString();

                var decValueWord1 = int.Parse(wordAddress, System.Globalization.NumberStyles.HexNumber);
                var decValueWord2 = int.Parse(wordAddress2, System.Globalization.NumberStyles.HexNumber);

                int directAdress = int.Parse(GetAdress(spaceSplitBeforeEqual[0]));

                newLine = $"#RetVal := BLKMOV(SRCBLK := P#M{directAdress + 1}.0 BYTE 1, DSTBLK => P#Q{decValueWord2}.0 BYTE  1);";
                newLine += $"\n#RetVal := BLKMOV(SRCBLK := P#M{directAdress}.0 BYTE 1, DSTBLK => P#Q{decValueWord2 + 1}.0 BYTE  1);";

                ConvertedProgram.Insert(3, $"VAR_TEMP\nRetVal: Int;\nEND_VAR");
            }
            else
            {
                if (Regex.IsMatch(newLine, @"([P][0-9][0-9A-Z][0-9A-Z][0-9A-Z])"))
                {
                    HandleTrf(index);
                    return;
                }

                //INPUT 
                if (Regex.IsMatch(newLine, @"([W][0-9][0-9A-Z][0-9A-Z][0-9A-Z])") && Regex.IsMatch(newLine, @"([E][0-9][0-9A-Z][0-9A-Z][0-9A-Z])")) // is het een woord dat naar een input schrijft?
                {
                    string wordAddress2 = spaceSplitBeforeEqual[2][1].ToString() + spaceSplitBeforeEqual[2][2].ToString() + spaceSplitBeforeEqual[2][3].ToString();

                    var decValueWord2 = int.Parse(wordAddress2, System.Globalization.NumberStyles.HexNumber);

                    int directAdress = int.Parse(GetAdress(spaceSplitAfterEqual[0]));

                    newLine = $"#RetVal := BLKMOV(SRCBLK := P#I{decValueWord2 + 1}.0 BYTE 1, DSTBLK => P#M{directAdress}.0 BYTE  1);";
                    newLine += $"\n#RetVal := BLKMOV(SRCBLK := P#I{decValueWord2}.0 BYTE 1, DSTBLK => P#M{directAdress + 1}.0 BYTE  1);";

                    ConvertedProgram.Insert(3, $"VAR_TEMP\nRetVal: Int;\nEND_VAR");
                }

            }

            //newLine = equalString[1] + equalString[0];

            Console.WriteLine(newLine);
            ConvertedProgram.Add(newLine);
        }

        private int HandleMr(int index)
        {
            int startLocation = index;
            int stopLocation = 0;
            List<string> tagsToBeReset = new List<string>();

            // einde MR bepalen
            for (int i = startLocation; i < ProgramToConvert.Count(); i++)
            {
                if (ProgramToConvert[i].Contains("finmr"))
                {
                    stopLocation = i;
                    break;
                }
            }

            var replacedString = ProgramToConvert[startLocation].Replace(" = mr", " @  mr");
            string[] equalString = replacedString.Split('@');
            equalString[0] = HandleBooInsideCondition(equalString[0]);
            ConvertedProgram.Add($"IF({ equalString[0]}) THEN           //START OF MasterRelay");

            //interne lus overlopen en behandelen:
            for (int i = startLocation + 1; i < stopLocation; i++)
            {
                //Gs variabelen opslaan
                if (ProgramToConvert[i].Contains("gs"))
                {
                    var gsString = ProgramToConvert[i].Replace(" = gs", " @  gs");
                    gsString = gsString.Replace("gs", "");
                    string[] gsParts = gsString.Split('@');
                    string[] commaSplit = gsParts[1].Split(',');
                    tagsToBeReset.Add($"\"{commaSplit[0].Trim()}\" := 0;");
                    tagsToBeReset.Add($"\"{commaSplit[1].Trim()}\" := 0;");

                }


                if (ProgramToConvert[i].Contains("fd"))
                {
                    //fd converteren : 
                    var fmString = ProgramToConvert[index].Replace(" = fd", " @  fd");
                    string[] fmEqualString = fmString.Split('@');
                    string[] commaSplit = fmEqualString[1].Split(',');
                    commaSplit[0] = commaSplit[0].Replace(" fd ", "");
                    fmEqualString[0] = HandleBooInsideCondition(fmEqualString[0]);

                    ConvertedProgram.Add($"{commaSplit[0].Trim()} := 0;");

                }

                if (ProgramToConvert[i].Contains("fm"))
                {
                    //fm converteren : 
                    var fdString = ProgramToConvert[index].Replace(" = fm", " @  fm");
                    string[] fdEqualSAtring = fdString.Split('@');
                    string[] commaSplit = fdEqualSAtring[1].Split(',');
                    commaSplit[0] = commaSplit[0].Replace(" fm ", "");
                    fdEqualSAtring[0] = HandleBooInsideCondition(fdEqualSAtring[0]);

                    ConvertedProgram.Add($"{commaSplit[0].Trim()} := 0;");

                }

                if (ProgramToConvert[i].Contains("boo") && !ProgramToConvert[i].Contains("gs") && !ProgramToConvert[i].Contains("fm") && !ProgramToConvert[i].Contains("fd")
                    && !ProgramToConvert[i].Contains("set") && !ProgramToConvert[i].Contains("reset"))
                {
                    var line = ProgramToConvert[i];
                    string beforeAccolade = "";
                    string beforeEqual = "";
                    string afterEqual = "";
                    if (line.Contains("{") && line.Contains("}"))
                    {

                        string[] splitString = line.Split('}');

                        beforeAccolade = splitString[0];
                        beforeAccolade = beforeAccolade.Replace("{", "(");
                        beforeAccolade = beforeAccolade.Replace("=", "@");
                        beforeAccolade += ")";
                        line = beforeAccolade + splitString[1];
                    }
                    if (line.Contains('='))
                    {
                        string[] splitStringWithEqual = line.Split('=');

                        beforeEqual = splitStringWithEqual[0];
                        afterEqual = splitStringWithEqual[1];

                    }


                    tagsToBeReset.Add($"\"{afterEqual.Trim()}\" := 0;");

                }

                //alle lijnen in MR lus converteren
                ConvertLine(ProgramToConvert[i], i);


            }

            ConvertedProgram.Add($"ELSE");
            ConvertedProgram.Add($"//ADD GS_CODE_RESETTING");
            ConvertedProgram.AddRange(tagsToBeReset);
            ConvertedProgram.Add($"END_IF;              //END OF MasterRelay");

            return stopLocation;
        }

        private void HandleLabel(int index)
        {
            var line = ProgramToConvert[index].Replace("label", "");
            line += ":";
            ConvertedProgram.Add(line);
        }


        private void HandleJmp(int index)
        {
            //GS converteren : 
            var replacedString = ProgramToConvert[index].Replace(" = jmp", " @  jmp");
            string[] equalString = replacedString.Split('@');
            equalString[1] = equalString[1].Replace(" jmp ", "GOTO ");
            equalString[0] = HandleBooInsideCondition(equalString[0]);

            ConvertedProgram.Add($"IF( {equalString[0]} ) THEN");
            ConvertedProgram.Add($"{equalString[1].Trim()};");
            ConvertedProgram.Add($"END_IF;");
        }

        private void HandleFd(int index)
        {
            //fm converteren : 
            var replacedString = ProgramToConvert[index].Replace(" = fd", " @  fd");
            string[] equalString = replacedString.Split('@');
            string[] commaSplit = equalString[1].Split(',');
            commaSplit[0] = commaSplit[0].Replace(" fd ", "");
            equalString[0] = HandleBooInsideCondition(equalString[0]);

            ConvertedProgram.Add($"{commaSplit[1].Trim()} := ( NOT {equalString[0].Trim()} AND {commaSplit[0].Trim()} );");
            ConvertedProgram.Add($"{commaSplit[0].Trim()} := {equalString[0].Trim()};");

        }

        private void HandleFm(int index)
        {
            //fm converteren : 
            var replacedString = ProgramToConvert[index].Replace(" = fm", " @  fm");
            string[] equalString = replacedString.Split('@');
            string[] commaSplit = equalString[1].Split(',');
            commaSplit[0] = commaSplit[0].Replace(" fm ", "");
            equalString[0] = HandleBooInsideCondition(equalString[0]);

            ConvertedProgram.Add($"{commaSplit[1].Trim()} := ( {equalString[0].Trim()} AND NOT {commaSplit[0].Trim()} );");
            ConvertedProgram.Add($"{commaSplit[0].Trim()} := {equalString[0].Trim()};");

        }

        private void HandleDeg(int index)
        {
            //  "W012A" := SHL(IN:= "W012A", N:= 1);: 
            //  deg W0164 , 16#C = W0164
            var replacedString = ProgramToConvert[index].Replace(" = ", " @  ");
            replacedString = replacedString.Replace("deg", "");
            string[] equalString = replacedString.Split('@');
            string[] commaSplit = equalString[0].Split(',');
            ConvertedProgram.Add($"{equalString[1]} := SHL(IN:= {commaSplit[0]}, N:= {commaSplit[1]});");

        }

        private void HandleDed(int index)
        {
            //  "W012A" := SHR(IN:= "W012A", N:= 1);:  
            //  ded W0164 , 16#C = W0164
            var replacedString = ProgramToConvert[index].Replace(" = ", " @  ");
            replacedString = replacedString.Replace("ded", "");
            string[] equalString = replacedString.Split('@');
            string[] commaSplit = equalString[0].Split(',');
            ConvertedProgram.Add($"{equalString[1]} := SHR(IN:= {commaSplit[0]}, N:= {commaSplit[1]});");

        }
        private void HandleCas(int index)
        {
            //  cas_de W0210
            //  CASE _variable_name_ OF
            var replacedString = ProgramToConvert[index].Replace("cas_de ", "");
            ConvertedProgram.Add($"CASE {replacedString.Trim()} OF");

        }
        private void HandleQuand(int index)
        {
            //  quand 16#1
            //  16#1:
            var replacedString = ProgramToConvert[index].Replace("quand ", "");
            ConvertedProgram.Add($"{replacedString.Trim()}:");

        }
        private void HandleAutres(int index)
        {
            //  autres
            //  ELSE ;
            ConvertedProgram.Add($"ELSE ;");

        }
        private void HandleFincas(int index)
        {
            //  cas_de W0210
            //  CASE _variable_name_ OF
            ConvertedProgram.Add($"END_CASE;");

        }

        private void HandleReset(int index)
        {
            var line = ProgramToConvert[index].Replace("boo", "");
            var splitLine = line.Split('=');
            splitLine[0] = HandleBooInsideCondition(splitLine[0]);
            ConvertedProgram.Add($"IF ( {splitLine[0]} ) THEN //{splitLine[1]}");
            ConvertedProgram.Add($"{splitLine[1].Replace("reset", "").Trim()} := 0;");
            ConvertedProgram.Add($"END_IF;");
        }

        private void HandleSet(int index)
        {
            var line = ProgramToConvert[index].Replace("boo", "");
            var splitLine = line.Split('=');
            splitLine[0] = HandleBooInsideCondition(splitLine[0]);
            ConvertedProgram.Add($"IF ( {splitLine[0]} ) THEN //{splitLine[1]}");
            ConvertedProgram.Add($"{splitLine[1].Replace("set", "").Trim()} := 1;");
            ConvertedProgram.Add($"END_IF;");

        }

        private void HandleTwb(int index)
        {
            var line = ProgramToConvert[index].Replace("twb ", "");
            var splitLine = line.Split('=');
            string[] commaSplit = splitLine[1].Split(',');

            //ConvertedProgram.Add($"{splitLine[0].Trim()} := 0;");
            for (int i = 0; i < commaSplit.Length; i++)
            {

                ConvertedProgram.Add($" {commaSplit[i].Trim()} := {splitLine[0].Trim()}.X{i};");
                //if (i > 7)
                //{
                //    //ConvertedProgram.Add($"{splitLine[1].Trim()}.X{i - 8} := {commaSplit[i].Trim()} ;");
                //    ConvertedProgram.Add($" {commaSplit[i].Trim()} := {splitLine[0].Trim()}.X{i-8};");
                //}
                //else
                //{
                //    //ConvertedProgram.Add($"{splitLine[1].Trim()}.X{i + 8} := {commaSplit[i].Trim()} ;");
                //    ConvertedProgram.Add($" {commaSplit[i].Trim()} := {splitLine[0].Trim()}.X{i+8};");
                //}


            }
        }

        private void HandleTbw(int index)
        {
            var line = ProgramToConvert[index].Replace("tbw ", "");
            var splitLine = line.Split('=');
            string[] commaSplit = splitLine[0].Split(',');

            ConvertedProgram.Add($"{splitLine[1].Trim()} := 0;");
            for (int i = 0; i < commaSplit.Length; i++)
            {
                ConvertedProgram.Add($"{splitLine[1].Trim()}.X{i} := {commaSplit[i].Trim()} ;");
                //if (i > 7)
                //{
                //    ConvertedProgram.Add($"{splitLine[1].Trim()}.X{i-8} := {commaSplit[i].Trim()} ;");
                //}
                //else
                //{
                //    ConvertedProgram.Add($"{splitLine[1].Trim()}.X{i + 8} := {commaSplit[i].Trim()} ;");
                //}


            }

        }

        private void HandleFinsi(int index)
        {
            var line = ProgramToConvert[index].Replace("finsi", "END_IF;");
            ConvertedProgram.Add(line);
        }

        private void HandleSinon(int index)
        {
            var line = ProgramToConvert[index].Replace("sinon", "ELSE");
            ConvertedProgram.Add(line);
        }

        private void HandleSi(int index)
        {
            var line = ProgramToConvert[index].Replace("si ", "IF ").Replace("{", "(").Replace("}", ")") + " THEN";
            line = HandleBooInsideCondition(line);
            ConvertedProgram.Add(line);
        }

        private void HandleLog(int index)
        {
            // logaritmische uitdrukking, bitwise vergelijkingen van AND, OR, XOR tussen woorden
            var line = ProgramToConvert[index].Replace("log ", "");
            var splitLine = line.Split('=');
            splitLine[0] = HandleBooInsideCondition(splitLine[0]);
            line = $"{splitLine[1]} := {splitLine[0]};";

            ConvertedProgram.Add(line);
        }



        private void HandleCal(int index)
        {
            var line = ProgramToConvert[index].Replace("cal ", "");
            var splitLine = line.Split('=');
            line = $"{splitLine[1]} := {splitLine[0]};";
            ConvertedProgram.Add(line);
        }

        private void HandleTrf(int index)
        {
            var line = ProgramToConvert[index].Replace("trf ", "");
            var splitLine = line.Split('=');

            if (line.Contains("T") && line.Contains("_"))
            {
                splitLine[1] = "//" + splitLine[1];
                splitLine[0] = splitLine[0] + "  NIET LANGER NODIG, PERIPH KAN RECHSTREEKS AANGESPROKEN WORDEN  ";

                line = $"{splitLine[1]} := {splitLine[0]};";
            }
            //CATCH Word (decimal) into time, but has to be *100 because old times were in .1 second units
            if (line.Contains("\"F") && line.Contains("\"W"))
            {
                string wordAddress = splitLine[1][3].ToString() + splitLine[1][4].ToString() + splitLine[1][5].ToString() + splitLine[1][6].ToString();

                var decValueWord = int.Parse(wordAddress, System.Globalization.NumberStyles.HexNumber);
                // "Timers".T[59].PT := "B0120";
                splitLine[1] = $"\"Timers\".T[{decValueWord}].PT";

                splitLine[0] = "WORD_TO_TIME(" + splitLine[0].Trim() + " * 100)";
                line = $"{splitLine[1]} := {splitLine[0]};";
                ConvertedProgram.Add(line);
            }
            if (line.Contains("\"F") && !line.Contains("\"W"))
            {
                string wordAddress = splitLine[1][3].ToString() + splitLine[1][4].ToString() + splitLine[1][5].ToString() + splitLine[1][6].ToString();

                var decValueWord = int.Parse(wordAddress, System.Globalization.NumberStyles.HexNumber);
                // "Timers".T[59].PT := "B0120";
                splitLine[1] = $"\"Timers\".T[{decValueWord}].PT";

                splitLine[0] = splitLine[0].Replace("10#", "T#").Trim() + "00ms";
                line = $"{splitLine[1]} := {splitLine[0]};";
                ConvertedProgram.Add(line.Replace("_", ".X"));
            }
        }

        private void HandleTra(int index)
        {
            var line = ProgramToConvert[index].Replace("tra ", "");

            if (line.Contains("dtb"))
            {
                // THIS IS A DECIMAL TO BINAIRY DECIMAL ENCODING
                // BCD32 want BCD16 gaat blijkbaar maar van -999 tot 999 => bijgevolg ook DINT-INT converting
                var splitLine = line.Replace("dtb", "@").Split('@');

                line = $"{splitLine[1].Trim()} := DINT_TO_INT(BCD32_TO_DINT({splitLine[0].Trim()}));";

            }
            if (line.Contains("btd"))
            {
                // THIS IS A BINAIRY TO DECIMAL ENCODING
                // BCD32 want BCD16 gaat blijkbaar maar van -999 tot 999 => bijgevolg ook DINT-INT converting
                var splitLine = line.Replace("btd", "@").Split('@');

                line = $"{splitLine[1].Trim()} := DINT_TO_BCD32(INT_TO_DINT({splitLine[0].Trim()}));";
            }

            ConvertedProgram.Add(line);
        }

        private void HandleVal(int index)
        {
            if (ProgramToConvert[index].Contains("finval"))
            {
                ConvertedProgram.Add(ProgramToConvert[index].Replace("finval", "END_IF;"));
            }
            else
            {
                var line = ProgramToConvert[index].Replace("boo", "IF(");
                line = HandleBooInsideCondition(line);
                ConvertedProgram.Add(line.Replace("= val", ") THEN"));
            }
        }

        private void HandleBoo(int index)
        {
            var line = ProgramToConvert[index];

            line = line.Replace("boo", "");
            string beforeAccolade = "";
            string beforeEqual = "";
            string afterEqual = "";
            if (line.Contains("{") && line.Contains("}"))
            {

                string[] splitString = line.Split('}');

                beforeAccolade = splitString[0];
                beforeAccolade = beforeAccolade.Replace("{", "( ");
                beforeAccolade = beforeAccolade.Replace("=", "@");
                beforeAccolade += " )";
                line = beforeAccolade + splitString[1];
            }
            if (line.Contains('='))
            {
                string[] splitStringWithEqual = line.Split('=');

                beforeEqual = splitStringWithEqual[0];
                afterEqual = splitStringWithEqual[1];

            }


            beforeEqual = beforeEqual.Replace("@", "=");

            beforeEqual = beforeEqual.Replace(".", "AND ").Replace("+", "OR ").Replace("OX", "XOR ").Replace("/", "NOT ").Replace("_", ".%X");

            if (afterEqual.Contains("tm"))
            {

                afterEqual = TimerConvert(afterEqual);

            }

            if (afterEqual.Contains("td"))
            {

                afterEqual = TimerConvert(afterEqual);
                beforeEqual = $"NOT( {beforeEqual.Trim()} )";
            }

            if (beforeEqual.Contains("\"T"))
            {

                beforeEqual = TimerConvertInsideCondition(beforeEqual);

            }
            line = afterEqual + " := " + beforeEqual + ";";
            ConvertedProgram.Add(line);

        }

        private void HandleGS(int index)
        {
            //GS converteren : 
            var replacedString = ProgramToConvert[index].Replace(" = gs", " @  gs");
            string[] equalString = replacedString.Split('@');
            string[] commaSplit = equalString[1].Split(',');
            commaSplit[0] = commaSplit[0].Replace(" gs ", "");
            equalString[0] = HandleBooInsideCondition(equalString[0]);

            ConvertedProgram.Add($"IF( {equalString[0].Trim()} AND {commaSplit[0].Trim()} ) THEN");
            ConvertedProgram.Add($"{commaSplit[0].Trim()} := 0;");
            ConvertedProgram.Add($"{commaSplit[1].Trim()} := 1;");
            ConvertedProgram.Add($"END_IF;");


        }

        private string HandleBooInsideCondition(string line)
        {

            line = line.Replace("boo", "");
            string beforeAccolade;
            if (line.Contains("{") && line.Contains("}"))
            {

                string[] splitString = line.Split('}');

                beforeAccolade = splitString[0];
                beforeAccolade = beforeAccolade.Replace("{", "( ");
                beforeAccolade += " )";
                line = beforeAccolade + splitString[1];
            }

            line = line.Replace(".", "AND ").Replace("+", "OR ").Replace("ox", "XOR ").Replace("/", "NOT ").Replace("_", ".X");

            if (line.Contains("\"T"))
            {

                line = TimerConvertInsideCondition(line);

            }


            return line;
        }
        private void AddVariables(int index)
        {
            var line = ProgramToConvert[index];
            string[] spaceSplit = line.Split(' ');

            for (int j = 0; j < spaceSplit.Length; j++)
            {
                if (spaceSplit[j].StartsWith("//"))
                {
                    break;
                }
                //if (spaceSplit[j].Contains("_") && spaceSplit[j].StartsWith("W"))
                //{
                //    spaceSplit[j] = spaceSplit[j].Replace("_",".X");
                //}
                if (spaceSplit[j].StartsWith("EC"))
                {
                    spaceSplit[j] = $"{spaceSplit[j]}();";
                }
                else if (spaceSplit[j].Length == 5 && Regex.Match(spaceSplit[j], @"\b([A-Z])([0-F])([0-F])([0-F])([0-F])").Success)
                {
                    spaceSplit[j] = $"\"{spaceSplit[j]}\"";
                }


            }
            ProgramToConvert[index] = (string.Join(" ", spaceSplit));
        }


        private string TimerConvertInsideCondition(string ToConvert)
        {
            var line = ToConvert;
            string[] spaceSplit = line.Split(' ');

            for (int j = 0; j < spaceSplit.Length; j++)
            {
                if (spaceSplit[j].StartsWith("\"T"))
                {
                    string wordAddress = spaceSplit[j][2].ToString() + spaceSplit[j][3].ToString() + spaceSplit[j][4].ToString() + spaceSplit[j][5].ToString();

                    var decValueWord = int.Parse(wordAddress, System.Globalization.NumberStyles.HexNumber);
                    // "Timers".T[59].IN := "B0120";
                    spaceSplit[j] = $"\"Timers\".T[{decValueWord}].Q";
                }

            }
            return string.Join(" ", spaceSplit);
        }

        private string TimerConvert(string ToConvert)
        {
            var line = ToConvert;
            string[] spaceSplit = line.Split(' ');

            for (int j = 0; j < spaceSplit.Length; j++)
            {

                if (spaceSplit[j].StartsWith("\"T"))
                {
                    string wordAddress = spaceSplit[j][2].ToString() + spaceSplit[j][3].ToString() + spaceSplit[j][4].ToString() + spaceSplit[j][5].ToString();

                    var decValueWord = int.Parse(wordAddress, System.Globalization.NumberStyles.HexNumber);
                    // "Timers".T[59].IN := "B0120";
                    ToConvert = $"\"Timers\".T[{decValueWord}].IN";
                }

            }

            return ToConvert;

        }

    }
}
