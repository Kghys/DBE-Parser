using ParsingLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParsingLib
{
    public class TagHelper
    {


        public List<String> Variables { get; set; }
        public List<Tag> TagList { get; set; }
        public List<String> Operands { get; set; }


        public TagHelper()
        {
            Variables = new List<string>();
            TagList = new List<Tag>();
            Operands = new List<string>();
        }

        public List<String> ConvertVariables(List<string> LinesToConvert)
        {

            for (int i = 3; i < LinesToConvert.Count(); i++)
            {
                AddVariables(LinesToConvert[i]);
            }

            return Variables;

        }


        private void AddVariables(string LineToConvert)
        {
            if (LineToConvert.Contains("(*"))
            {
                return;
            }
            string[] spaceSplit = LineToConvert.Split(' ');


            for (int j = 0; j < spaceSplit.Length; j++)
            {

                if (spaceSplit[j].Length == 5 && Regex.Match(spaceSplit[j], @"\b([A-Z])([0-F])([0-F])([0-F])([0-F])").Success)
                {
                    if (!Variables.Contains(spaceSplit[j]))
                    {
                        Variables.Add(spaceSplit[j]);
                        MakeTag(spaceSplit[j]);
                    }

                }
                else if (spaceSplit[j].Count() >= 2 && Regex.Match(spaceSplit[j], @"^[a-zA-Z]+$").Success)
                {
                    if (!Operands.Contains(spaceSplit[j].Trim().ToLower()))
                    {

                        Operands.Add(spaceSplit[j].Trim().ToLower());

                    }


                }


            }

        }

        private void MakeTag(string TagName)
        {
            Tag tagToBeAdded;
            string wordAddress = TagName[1].ToString() + TagName[2].ToString() + TagName[3].ToString() + TagName[4].ToString();
            string boolAddress = TagName[1].ToString() + TagName[2].ToString() + TagName[3].ToString();

            var decValueWord = int.Parse(wordAddress, System.Globalization.NumberStyles.HexNumber);
            var decValueBool = int.Parse(boolAddress, System.Globalization.NumberStyles.HexNumber);

            switch (TagName[0])
            {
                case 'B':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%M{decValueBool}.{TagName[4]}" };
                    break;
                case 'W':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "Word", LogicalAddress = $"%MW{decValueWord}" };
                    break;
                case 'J':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "Byte", LogicalAddress = $"%MB{decValueWord}" };
                    break;
                case 'H':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "Byte", LogicalAddress = $"%MB{decValueWord + 1 }" };
                    break;
                case 'D':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "Dint", LogicalAddress = $"%MW{decValueWord}" };
                    break;
                case 'F':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "Word", LogicalAddress = $"%MW{decValueWord}" };
                    break;
                case 'V':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "Word", LogicalAddress = $"%MW{decValueWord}" };
                    break;
                case 'E':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%I{decValueBool}.{TagName[4]}" };
                    break;
                case 'A':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%Q{decValueBool}.{TagName[4]}" };
                    break;
                case 'P':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "byte", LogicalAddress = $"%MB{decValueWord}" };
                    break;
                default:
                    tagToBeAdded = null;
                    break;
            }

            if (tagToBeAdded != null)
            {

                TagList.Add(tagToBeAdded);
            }

        }

    }

}
