using System.Collections.Generic;
using PDM.BusinessData;
using PDM.BusinessData.Service;
using PDM.BusinessLogic.Service;
using PDM.Common.Interfaces;

namespace PDM.BusinessLogic
{
    public class BusinessService : IBusinessService
    {
        private DataService _Service;

        /// <summary>
        /// Gets the service.
        /// </summary>
        private IDataService Service
        {
            get { return _Service ?? (_Service = new DataService()); }
        }

        public IList<IFile> GetFiles()
        {
            return Service.GetFiles();
        }

        public void DeleteFile(string filePath)
        {
            Service.DeleteFile(filePath);
        }

        public void CreateOrUpdateFileAsync(IFile file)
        {
            Service.CreateOrUpdateFileAsync(file);
        }
    }
}
