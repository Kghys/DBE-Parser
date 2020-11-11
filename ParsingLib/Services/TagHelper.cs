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


        public TagHelper()
        {
            Variables = new List<string>();
            TagList = new List<Tag>();
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


            }

        }

        private void MakeTag(string TagName)
        {
            Tag tagToBeAdded;
            string tagAddress = TagName[1].ToString() + TagName[2].ToString() + TagName[3].ToString();

            switch (TagName[0])
            {
                case 'B': tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%M{tagAddress}.{TagName[4]}" };
                    break;
                case 'W':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%MW{tagAddress + TagName[4].ToString()}" };
                    break;
                case 'E':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%I{tagAddress}.{TagName[4]}" };
                    break;
                case 'A':
                    tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%Q{tagAddress}.{TagName[4]}" };
                    break;
                default: tagToBeAdded = new Tag { Name = TagName, DataType = "bool", LogicalAddress = $"%M{tagAddress}.{TagName[4]}" };

                    break;
            }



            TagList.Add(tagToBeAdded);

        }

    }

}
