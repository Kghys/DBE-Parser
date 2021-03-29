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
        public List<List<String>> MatrixList { get; set; }


        public TagHelper()
        {
            Variables = new List<string>();
            TagList = new List<Tag>();
            Operands = new List<string>();
            MatrixList = new List<List<string>>();
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

                if (/*spaceSplit[j].Length == 5 &&*/ Regex.Match(spaceSplit[j], @"([A-Z])([0-F])([0-F])([0-F])([0-F])").Success)
                {
                    if (!Variables.Contains(spaceSplit[j]))
                    {
                        Variables.Add(spaceSplit[j]);
                        
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

        public void MakeTags()
        {
            Variables.Sort();

            foreach (var item in Variables)
            {
                MakeTag(item);
            }
        }


        private void MakeTag(string TagName)
        {
            Tag tagToBeAdded;
            string wordAddress = TagName[1].ToString() + TagName[2].ToString() + TagName[3].ToString() + TagName[4].ToString();
            string boolAddress = TagName[1].ToString() + TagName[2].ToString() + TagName[3].ToString();
            var decValueBool = 0; var decValueWord = 0;
            bool v = int.TryParse(wordAddress, System.Globalization.NumberStyles.HexNumber, null, out decValueWord);
            bool w = int.TryParse(boolAddress, System.Globalization.NumberStyles.HexNumber, null, out decValueBool);

            switch (TagName[0])
            {
                case 'B':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%M{decValueBool + 2}.{TagName[4]}" };
                    break;
                case 'W':
                    var newTagListW = new List<string> { TagName, $"%MW{decValueWord}" };
                    MatrixList.Add(newTagListW);
                    var offset = MatrixList.IndexOf(newTagListW);
                    tagToBeAdded = new Tag { Name = TagName, DataType = "Word", LogicalAddress = $"%MW{decValueWord + 5000 + (offset)}" };
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
                    var newTagListF = new List<string> { TagName, $"%MW{decValueWord}" };
                    MatrixList.Add(newTagListF);
                    var offsetF = MatrixList.IndexOf(newTagListF);
                    tagToBeAdded = new Tag { Name = TagName, DataType = "Word", LogicalAddress = $"%MW{decValueWord + 9000 + (offsetF)}" };
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
                case 'T':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "", LogicalAddress = $"" };
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
