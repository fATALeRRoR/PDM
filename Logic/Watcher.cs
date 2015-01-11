using System.IO;

namespace PDM.BusinessLogic
{
    public class Watcher
    {
        private readonly FileSystemWatcher _Watcher;

        #region Events

        public event FileSystemEventHandler FileCreated
        {
            add
            {
                _Watcher.Created += value;
            }
            remove
            {
                _Watcher.Created -= value;
            }
        }

        public event RenamedEventHandler FileRenamed
        {
            add
            {
                _Watcher.Renamed += value;
            }
            remove
            {
                _Watcher.Renamed -= value;
            }
        }

        public event FileSystemEventHandler FileDeleted
        {
            add
            {
                _Watcher.Deleted += value;
            }
            remove
            {
                _Watcher.Deleted -= value;
            }
        }

        public event FileSystemEventHandler FileChanged
        {
            add
            {
                _Watcher.Changed += value;
            }
            remove
            {
                _Watcher.Changed -= value;
            }
        }

        #endregion

        public Watcher(string folderPath)
        {
            _Watcher = new FileSystemWatcher(folderPath);
            _Watcher.Filter = string.Empty;
            _Watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName
                                   | NotifyFilters.Size;
            _Watcher.IncludeSubdirectories = true;

            //Begin watching
            _Watcher.EnableRaisingEvents = true;
        }
    }
}
