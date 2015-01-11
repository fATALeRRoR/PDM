using System;
using System.Linq;
using PDM.Common.Constants;

namespace PDM.BusinessData.Actions
{
    class DeleteFileAction
    {
        private readonly string _FilePath;

        public DeleteFileAction(string filePath)
        {
            _FilePath = filePath;
        }

        public void Execute()
        {
            var r = new Deserialize().Execute();

            var file = r.Files.FirstOrDefault(k => k.FilePath.Equals(_FilePath, Const.DefaultStringComparison));

            if (file != null)
            {
                r.Files.Remove(file);
                new Serialize(r).Execute();    
            }          
        }
    }
}
