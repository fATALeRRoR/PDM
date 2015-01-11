using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PDM.BusinessData.Serializer
{
    [XmlRoot(ElementName = "Root", IsNullable = false)]
    [Serializable]
    public class Root
    {
        public Root()
        {
            Files = new List<File>();
        }

        [XmlArray("Files"), XmlArrayItem("File", typeof(File))]
        public List<File> Files { get; set; }
    }
}