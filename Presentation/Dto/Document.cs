using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using PDM.Common.Interfaces;

namespace PDM.Client.Dto
{
    [DebuggerDisplay("File={FileName},Date={Date},Descr={Description},Project={Project},Owner={Owner},KeywordsString={KeywordsString}")]
    public class Document : INotifyPropertyChanged, IDocument
    {
        public Document()
        {
            Keywords = new ObservableCollection<string>();
            _Keywords.CollectionChanged += (sender, args) =>
            {
                UpdateKeywordsString();
            };
        }

        #region Constants

        #endregion

        private ImageSource _Icon;

        /// <summary>
        /// File icon
        /// </summary>
        public ImageSource Icon
        {
            get { return _Icon; }
            set
            {
                _Icon = value;
                OnPropertyChanged();
            }
        }

        private string _FilePath;

        /// <summary>
        /// Full path to file
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
                OnPropertyChanged();
            }
        }

        private string _FileName;

        /// <summary>
        /// File name with extension
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                OnPropertyChanged();
            }
        }

        private string _FolderPath;

        /// <summary>
        /// FolderPath in which file is
        /// </summary>
        public string FolderPath
        {
            get { return _FolderPath; }
            set
            {
                _FolderPath = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _Date;

        /// <summary>
        /// File creation date
        /// </summary>
        public DateTime? Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                OnPropertyChanged();

                if (Date.HasValue)
                {
                    DateString = Date.Value.ToShortDateString();
                }
                else
                {
                    DateString = null;
                }
            }
        }

        private string _DateString;
        public string DateString
        {
            get { return _DateString; }
            private set
            {
                _DateString = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _LastAccessTime;
        public DateTime? LastAccessTime
        {
            get { return _LastAccessTime; }
            set
            {
                _LastAccessTime = value;
                OnPropertyChanged();

                if (LastAccessTime.HasValue)
                {
                    LastAccessTimeString = LastAccessTime.Value.ToString("G");
                }
                else
                {
                    LastAccessTimeString = null;
                }
            }
        }

        private string _LastAccessTimeString;
        public string LastAccessTimeString
        {
            get { return _LastAccessTimeString; }
            private set
            {
                _LastAccessTimeString = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _LastWriteTime;
        public DateTime? LastWriteTime
        {
            get { return _LastWriteTime; }
            set
            {
                _LastWriteTime = value;
                OnPropertyChanged();

                if (LastWriteTime.HasValue)
                {
                    LastWriteTimeString = LastWriteTime.Value.ToString("G");
                }
                else
                {
                    LastWriteTimeString = null;
                }
            }
        }

        private string _LastWriteTimeString;
        public string LastWriteTimeString
        {
            get { return _LastWriteTimeString; }
            private set
            {
                _LastWriteTimeString = value;
                OnPropertyChanged();
            }
        }

        private string _Description;
        /// <summary>
        /// File description
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                OnPropertyChanged();
            }
        }

        private bool _IsDeleted;
        /// <summary>
        /// Is deleted?
        /// </summary>
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set
            {
                _IsDeleted = value;
                OnPropertyChanged();
            }
        }

        private string _Project;
        /// <summary>
        /// Project, to which document belongs
        /// </summary>
        public string Project
        {
            get { return _Project; }
            set
            {
                _Project = value;
                OnPropertyChanged();
            }
        }

        private string _Owner;
        /// <summary>
        /// Document owner
        /// </summary>
        public string Owner
        {
            get { return _Owner; }
            set
            {
                _Owner = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _Keywords;
        /// <summary>
        /// Document keywords
        /// </summary>
        public ObservableCollection<string> Keywords
        {
            get { return _Keywords; }
            set
            {
                _Keywords = value;
                OnPropertyChanged();
            }
        }

        private void UpdateKeywordsString()
        {
            KeywordsString = string.Join(", ", Keywords);
        }

        private string _KeywordsString;
        public string KeywordsString
        {
            get { return _KeywordsString; }
            private set
            {
                _KeywordsString = value;
                OnPropertyChanged();
            }
        }

        List<string> IFile.Keywords
        {
            get { return Keywords.ToList(); }
            set
            {
                foreach (var v in value)
                {
                    Keywords.Add(v);
                }               
            }
        }



        private string _FileExtension;

        /// <summary>
        /// File name extension
        /// </summary>
        public string FileExtension
        {
            get { return _FileExtension; }
            set
            {
                _FileExtension = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string status = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(status));
            }
        }

        #endregion
    }
}