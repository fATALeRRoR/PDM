using System;
using System.Collections.Generic;

namespace PDM.Common.Interfaces
{
    public interface IFile
    {
        string FilePath { get; set; }
        DateTime? Date { get; set; }
        string Description { get; set; }
        string Project { get; set; }
        string Owner { get; set; }
        List<string> Keywords { get; set; }
    }
}
