using System;

namespace PDM.Common.Interfaces
{
    public interface IDocument : IFile
    {
        string Description { get; set; }
        string Owner { get; set; }
        string Project { get; set; }

        string FileName { get; set; }
        string FileExtension { get; set; }
        string FolderPath { get; set; }
        bool IsDeleted { get; set; }
        string KeywordsString { get;  }   
    
        /// <summary>
        /// Business date
        /// </summary>
        DateTime? Date { get; set; }
        string DateString { get; }
        DateTime? LastAccessTime { get; set; }
        string LastAccessTimeString { get;  }
        DateTime? LastWriteTime { get; set; }
        string LastWriteTimeString { get; }
    }
}