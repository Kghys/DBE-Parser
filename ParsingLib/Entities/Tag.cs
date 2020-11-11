using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib.Entities
{
    public class Tag
    {
        public string Name { get; set; }
        public string Path { get; set; } = "DefaultTagTable";
        public string DataType { get; set; }
        public string LogicalAddress { get; set; }
        public string Comment { get; set; } = "";
        public string HmiVisible { get; set; } = "true";
        public string HmiAcces { get; set; } = "true";
        public string HmiWriteable { get; set; } = "true";
        public string TypeObjectId { get; set; } = "";
        public string VersionId { get; set; } = "";
                     
    }
}
