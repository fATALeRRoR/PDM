using System.Collections.Generic;
using PDM.BusinessData.Dto;
using PDM.BusinessData.Serializer;
using PDM.Common.Interfaces;

namespace PDM.BusinessData.Actions
{
    class GetFilesAction
    {
        public List<IFile> Execute()
        {
            var r = new Deserialize().Execute();

            return r.Files.ConvertAll(Convert);
        }

        private IFile Convert(File from)
        {
            IFile to = new FileDto();
            to.FilePath = @from.FilePath;
            to.Date = @from.Date;
            to.Description = @from.Description;
            to.Project = @from.Project;
            to.Owner = @from.Owner;
            to.Keywords = @from.Keywords;

            return to;
        }
    }
}
