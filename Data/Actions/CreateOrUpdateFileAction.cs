using System;
using System.Linq;
using PDM.BusinessData.Serializer;
using PDM.Common.Constants;
using PDM.Common.Interfaces;

namespace PDM.BusinessData.Actions
{
    class CreateOrUpdateFileAction
    {
        private readonly IFile _File;

        public CreateOrUpdateFileAction(IFile file)
        {
            _File = file;
        }

        public void Execute()
        {
            var r = new Deserialize().Execute();

            var file = r.Files.FirstOrDefault(k => k.FilePath.Equals(_File.FilePath, Const.DefaultStringComparison));

            if (file != null)
            {
                //Update
                UpdateFile(file, _File);
            }
            else
            {
                //New
                r.Files.Add(CreateFile(_File));
            }

            new Serialize(r).Execute();        
        }

        void UpdateFile(File to, IFile @from)
        {
            to.Date = @from.Date;
            to.Description = @from.Description;
            to.Project = @from.Project;
            to.Owner = @from.Owner;
            to.Keywords = @from.Keywords;
        }

        File CreateFile(IFile data)
        {
            var file = new File {FilePath = data.FilePath};
            UpdateFile(file, data);

            return file;
        }
    }
}
