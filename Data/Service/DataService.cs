using System.Collections.Generic;
using PDM.BusinessData.Actions;
using PDM.BusinessData.Service;
using PDM.Common.Interfaces;

namespace PDM.BusinessData
{
    public class DataService : IDataService
    {
        public List<IFile> GetFiles()
        {
            return new GetFilesAction().Execute();
        }

        public void DeleteFile(string filePath)
        {
            new DeleteFileAction(filePath).Execute();
        }

        public void CreateOrUpdateFileAsync(IFile file)
        {
            new CreateOrUpdateFileAction(file).Execute();            
        }
    }
}
