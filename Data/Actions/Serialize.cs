using PDM.BusinessData.Constants;
using PDM.BusinessData.Serializer;
using PDM.Common;

namespace PDM.BusinessData.Actions
{
    class Serialize
    {
        private readonly Root _Root;

        public Serialize(Root root)
        {
            _Root = root;
        }

        public void Execute()
        {
            new BackupFileAction().Execute();
            new DataSerializer().SerializeToFile(_Root, Files.DataFile);   
        }
    }
}
