using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using PDM.Client.Dto;

namespace PDM.Client.Extensions
{
    public static class Common
    {
        public static List<string> ToList(this StringCollection source)
        {
            var to = new List<string>();

            if (source != null)
            {
                to.AddRange(source.Cast<string>());
            }

            return to;
        }

        public static ObservableCollection<string> ToObservableCollectionList(this StringCollection source)
        {
            var to = new ObservableCollection<string>();

            if (source != null)
            {
                to.AddRange(source.Cast<string>());
            }

            return to;
        }

        public static void AddRange(this ObservableCollection<string> source, IEnumerable<string> list)
        {
            foreach (var l in list)
            {
                source.Add(l);
            }
        }

        public static Document Copy(this Document from)
        {
            var to = new Document();
            to.Date = from.Date;
            to.Description = from.Description;
            to.FileExtension = from.FileExtension;
            to.FileName = from.FileName;
            to.FilePath = from.FilePath;
            to.FolderPath = from.FolderPath;
            to.Icon = from.Icon;
            to.IsDeleted = from.IsDeleted;
            to.Keywords.AddRange(from.Keywords);
            to.LastAccessTime = from.LastAccessTime;
            to.LastWriteTime = from.LastWriteTime;
            to.Owner = from.Owner;
            to.Project = from.Project;            
            return to;
        }
    }
}
