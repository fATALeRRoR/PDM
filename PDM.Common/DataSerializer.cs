using System.IO;
using System.Xml.Serialization;

namespace PDM.Common
{
    public class DataSerializer
    {
        public void SerializeToFile<T>(T obj, string filePath)
        {
            var xsSubmit = new XmlSerializer(typeof(T));

            using (var sww = new StreamWriter(filePath))
            {
                xsSubmit.Serialize(sww, obj);
            }
        }

        public T DeserializeFromFile<T>(string filePath)
        {
            var xsSubmit = new XmlSerializer(typeof(T));

            using (var sww = new StreamReader(filePath))
            {
                return (T) xsSubmit.Deserialize(sww);
            }
        }
   }
}
