using PDM.BusinessData.Constants;
using PDM.BusinessData.Serializer;
using PDM.Common;
using File = System.IO.File;

namespace PDM.BusinessData.Actions
{
    class Deserialize
    {
        public Root Execute()
        {
            if (File.Exists(Files.DataFile))
            {
                return new DataSerializer().DeserializeFromFile<Root>(Files.DataFile);
            }

            return new Root();            
        }
    }
}
