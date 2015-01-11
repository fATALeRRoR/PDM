using System.Collections.Generic;
using PDM.Common.Interfaces;

namespace PDM.BusinessLogic.Service
{
    public interface IBusinessService
    {
        IList<IFile> GetFiles();
        void DeleteFile(string filePath);
        void CreateOrUpdateFileAsync(IFile file);
    }
}