using System;
using System.Collections.Generic;
using PDM.Common.Interfaces;

namespace PDM.BusinessData.Dto
{
    public class FileDto : IFile
    {
        public string FilePath { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string Project { get; set; }
        public string Owner { get; set; }
        public List<string> Keywords { get; set; }
    }
}