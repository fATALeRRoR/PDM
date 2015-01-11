using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDM.BusinessData.Serializer
{  
    [Serializable]
    public class File
    {
        [XmlElement("FilePath")]
        public string FilePath { get; set; }

        [XmlElement("Date", DataType = "dateTime")]
        public DateTime? Date { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("Project")]
        public string Project { get; set; }

        [XmlElement("Owner")]
        public string Owner { get; set; }

        [XmlArray("Keywords"), XmlArrayItem("Keyword", typeof(string))]
        public List<string> Keywords { get; set; }
    }
}