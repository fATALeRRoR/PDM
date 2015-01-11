using System.Collections.Generic;
using PDM.Common.Interfaces;

namespace PDM.BusinessData.Service
{
    public interface IDataService
    {
        List<IFile> GetFiles();
        void DeleteFile(string filePath);
        void CreateOrUpdateFileAsync(IFile file);
    }
}
