using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;               // For prcesss related information
using System.Runtime.InteropServices;   // For DLL importing 
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using PDM.BusinessLogic;
using PDM.BusinessLogic.Service;
using PDM.Client.Common;
using PDM.Client.Controls;
using PDM.Client.Dto;
using PDM.Client.Extensions;
using PDM.Client.Helpers;
using PDM.Client.Settings;
using PDM.Common.Constants;
using PDM.Common.Interfaces;
using ContextMenu = System.Windows.Controls.ContextMenu;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace PDM.Client.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constants

        /// <summary>
        /// 0
        /// </summary>
        private const int SW_HIDE = 0;        

        #endregion

        #region Private

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        #endregion

        #region Commands

        public Command NewCommand { get; set; }
        public Command ViewCommand { get; set; }
        public Command EditCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command SendEmailCommand { get; set; }
        public Command PrintCommand { get; set; }
        public Command ClearFilterCommand { get; set; }
        public Command SettingsCommand { get; set; }
        public Command AboutCommand { get; set; }

        #endregion
        
        private AGridViewColumn _LastSortColumn;
        private readonly Dictionary<string, ListSortDirection> _LastSortings;

        public MainWindow()
        {
            InitializeComponent();

            if (ApplicationSettings.SerializedSetings.IsWindowMaximized)
            {
                WindowState = WindowState.Maximized;                
            }

            Top = ApplicationSettings.SerializedSetings.WindowTop;
            Left = ApplicationSettings.SerializedSetings.WindowLeft;

            for (int i = 0; i < ApplicationSettings.FilterTexts.Count; i++)
            {
                var filterName = ApplicationSettings.FilterNames[i];
                var filterText = ApplicationSettings.FilterTexts[i];
                var filterIconPath = ApplicationSettings.FilterIcons[i];

                var b = new AButton();
                b.Number = i + 1;
                b.FilterName = filterName;
                b.FilterText = filterText;
                b.IconPath = filterIconPath;
                b.Click += ShortcutButton_Click;

                var menu = new ContextMenu();
                var mi1 = new MenuItem();
                mi1.Header = "Change icon";
                mi1.Click += MenuIconChange_Click;
                menu.Items.Add(mi1);
                var mi2 = new MenuItem();
                mi2.Header = "Change name";
                mi2.Click += MenuFilterNameChange_Click;
                menu.Items.Add(mi2);

                var mi3 = new MenuItem();
                mi3.Header = "Save filter";
                mi3.Click += MenuFilterTextChange_Click;
                menu.Items.Add(mi3);

                b.ContextMenu = menu;

                ShotcutsPanel.Children.Add(b);
            }

            //Set data context
            DataContext = this;

            Service = new BusinessService();

            DocumentsDic = new Dictionary<string, Document>(Const.DefaultStringComparer);
            Documents = new ObservableCollection<Document>();
            Documents.CollectionChanged += Documents_CollectionChanged;

            //Actions
            NewCommand = new Command(NewExecute);
            ViewCommand = new Command(ViewExecute, ViewCanExecute);
            EditCommand = new Command(EditExecute, EditCanExecute);
            DeleteCommand = new Command(DeleteExecute, DeleteCanExecute);
            SendEmailCommand = new Command(SendEmailExecute, SendEmailCanExecute);
            PrintCommand = new Command(PrintExecute, PrintCanExecute);
            ClearFilterCommand = new Command(ClearFilterExecute);
            SettingsCommand = new Command(SettingsExecute);
            AboutCommand = new Command(AboutExecute);
            
            //Sorting initialize
            _LastSortings = new Dictionary<string, ListSortDirection>();
            _LastSortings.Add(Constants.DateProperty, ListSortDirection.Descending);
            _LastSortings.Add(Constants.DescriptionProperty, ListSortDirection.Descending);
            _LastSortings.Add(Constants.FileNameExtensionProperty, ListSortDirection.Descending);
            _LastSortings.Add(Constants.FileNameProperty, ListSortDirection.Descending);
            _LastSortings.Add(Constants.FolderPathProperty, ListSortDirection.Descending);
            _LastSortings.Add(Constants.KeywordsProperty, ListSortDirection.Descending);
            _LastSortings.Add(Constants.OwnerProperty, ListSortDirection.Descending);
            _LastSortings.Add(Constants.ProjectProperty, ListSortDirection.Descending);

            GridView.LoadSettings();

            if (!ApplicationSettings.SerializedSetings.IsSortedAsc)
            {
                _LastSortings[ApplicationSettings.SerializedSetings.SortColumnName] = ListSortDirection.Ascending;                
            }

            var sortColumn = GridView.Columns.Cast<AGridViewColumn>()
                .First(k => k.SortPropertyName.Equals(ApplicationSettings.SerializedSetings.SortColumnName));
            SetSortingOnColumn(sortColumn);

            //Sets focus to filter
            filterTextBox.Focus();
        }

        #region Properties

        public IBusinessService Service { get; set; }

        private Document _SelectedDocument;
        public Document SelectedDocument
        {
            get { return _SelectedDocument; }
            set
            {
                if (_SelectedDocument != value)
                {
                    _SelectedDocument = value;
                    OnPropertyChanged();

                    ViewCommand.RaiseCanExecuteChanged();
                    EditCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                    SendEmailCommand.RaiseCanExecuteChanged();
                    PrintCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private ObservableCollection<Document> _Documents;
        /// <summary>
        /// Gets or sets the documents.
        /// </summary>
        /// <value>
        /// The documents.
        /// </value>
        public ObservableCollection<Document> Documents
        {
            get { return _Documents; }
            set
            {
                if (_Documents != value)
                {
                    _Documents = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<string, Document> DocumentsDic { get; set; }

        private int _ShownDocumentsCount;
        public int ShownDocumentsCount
        {
            get { return _ShownDocumentsCount; }
            set
            {
                if (_ShownDocumentsCount != value)
                {
                    _ShownDocumentsCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _DocumentsTotalCount;

        public int DocumentsTotalCount
        {
            get { return _DocumentsTotalCount; }
            set
            {
                if (_DocumentsTotalCount != value)
                {
                    _DocumentsTotalCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _StatusBarMessage;
        public string StatusBarMessage
        {
            get { return _StatusBarMessage; }
            set
            {
                _StatusBarMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _IsInProgress;

        public bool IsInProgress
        {
            get
            {
                return _IsInProgress;
            }
            set
            {
                _IsInProgress = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Actions

        private void NewExecute()
        {
            var window = new AddDocumentWindow();
            window.Owner = this;

            if (SelectedDocument != null)
            {
                window.OriginalFilePath = SelectedDocument.FilePath;
                window.IsCopyFromFileSelected = true;
                window.Folder = SelectedDocument.FolderPath;
                window.FileName = SelectedDocument.FileName;
                window.Date = SelectedDocument.Date.GetValueOrDefault(DateTime.Today);
                window.FileDate = window.Date;
                window.Description = SelectedDocument.Description;
                window.Project = SelectedDocument.Project;                
                window.DocumentOwner = SelectedDocument.Owner;
                window.Keywords.AddRange(SelectedDocument.Keywords);                
            }

            window.LoadProjectsForAutoComplete(
                Documents.ToList()
                    .Select(k => k.Project)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .OrderBy(t => t));

            if (window.ShowDialog().GetValueOrDefault())
            {
                ReloadDocument(window.SavedDocument.FilePath);
                filterTextBox.Text = window.FileName;

                ViewDocument(window.SavedDocument.FilePath);
            }
        }

        private void ViewExecute()
        {
            ViewDocument(string.Format(SelectedDocument.FilePath));
        }

        private bool ViewCanExecute()
        {
            return SelectedDocument != null && !SelectedDocument.IsDeleted;
        }

        private void EditExecute()
        {
            var copy = SelectedDocument.Copy();
            var window = new EditDocumentWindow();
            window.Document = copy;
            window.FileDate = copy.Date.GetValueOrDefault(DateTime.Today);
            window.LoadProjectsForAutoComplete(
                Documents.ToList()
                    .Select(k => k.Project)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .OrderBy(t => t));
            window.Owner = this;
            if (window.ShowDialog().GetValueOrDefault())
            {
                ReloadDocument(SelectedDocument.FilePath);
            }
        }

        private bool EditCanExecute()
        {
            return SelectedDocument != null;
        }

        private void DeleteExecute()
        {
            var result = new MessageBoxDialog("Delete", "Do you want delete documents (including  physical file)?", MessageBoxButton.YesNo).Show();

            if (result == MessageBoxResult.Yes || result == MessageBoxResult.OK)
            {
                var docs = GetSelectedDocuments();

                foreach (var doc in docs)
                {
                    Service.DeleteFile(doc.FilePath);

                    if (File.Exists(doc.FilePath))
                    {
                        File.Delete(doc.FilePath);
                    }

                    RemoveDocument(doc);
                }                
            }
        }

        private bool DeleteCanExecute()
        {
            var docs = GetSelectedDocuments();
            return docs != null && docs.Any();
        }

        private void SendEmailExecute()
        {
            var paths = GetSelectedDocuments().Select(k => k.FilePath).ToList();            

            foreach (var filePath in paths)
            {
                if (!File.Exists(filePath))
                {
                    ShowFileNotFoundDialog(filePath);
                    return;
                }
            }

            new MicrosoftOutlook().Start(paths);            
        }

        private bool SendEmailCanExecute()
        {
            return SelectedDocument != null;
        }

        private void PrintExecute()
        {
            if (File.Exists(SelectedDocument.FilePath))
            {
                PrintDocument(SelectedDocument.FilePath);
            }
            else
            {
                ShowFileNotFoundDialog(SelectedDocument.FilePath);
            }
        }

        private bool PrintCanExecute()
        {
            return SelectedDocument != null;
        }

        private void ClearFilterExecute()
        {
            ClearFilter();
        }

        private void SettingsExecute()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            var result = settingsWindow.ShowDialog();

            if (result.GetValueOrDefault())
            {
                StartFullScan();
            }
        }

        private void AboutExecute()
        {
            var window = new AboutWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        #endregion

        #region Methods

        private async void UpdateDocumentInfoAsync(Document document)
        {
            await Task.Run(() =>
            {
                var info = new FileInfo(document.FilePath);

                if (!document.Date.HasValue)
                {
                    document.Date = info.CreationTime;
                }

                document.LastAccessTime = info.LastAccessTime;
                document.LastWriteTime = info.LastWriteTime;
                document.IsDeleted = !info.Exists;

                if (!document.IsDeleted)
                {
                    document.Icon = System.Drawing.Icon.ExtractAssociatedIcon(document.FilePath).ToImageSource();
                }
                else
                {
                    DeletedFileHelper.AddDeletedKeyword(document);                    
                }
            });
        }

        void Documents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Document doc in e.NewItems)
                    {
                        UpdateDocumentInfoAsync(doc);
                        DocumentsDic.Add(doc.FilePath, doc);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Document doc in e.OldItems)
                    {
                        DocumentsDic.Remove(doc.FilePath);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateDocumentsCountInStatusBar();
        }



        private void ShowMessageInStatusBar(string message)
        {
            StatusBarMessage = message;
        }

        private void LoadSavedDocuments()
        {
            var files = Service.GetFiles();
            foreach (var file in files)
            {
                AddDocument(Convert(file));
            }
        }

        private Document Convert(IFile file)
        {
            var d = ConvertToDto(file.FilePath);
            d.Date = file.Date;
            d.Description = file.Description;
            d.Project = file.Project;
            d.Owner = file.Owner;
            d.Keywords.AddRange(file.Keywords);
            return d;
        }

        private void StartFullScan()
        {
            var folders = ApplicationSettings.Folders.ToList();            

            foreach (var folder in folders)
            {
                string f = folder;

                if (Directory.Exists(f))
                {
                    ScanDirectoryAsync(f, GetFileExtensionsToScan().Select(k => "*"+k).ToList());
                }
            }
        }

        /// <summary>
        /// *.doc, *.docx, etc
        /// </summary>
        /// <returns></returns>
        private List<string> GetFileExtensionsToScan()
        {
            return ApplicationSettings.FileTypes.ToList().Select(k => "." + k).ToList();
        }

        private async void ScanDirectoryAsync(string folderPath, List<string> extensions)
        {
            await Task.Run(() =>
            {
                ShowMessageInStatusBar(string.Format("Started scanning folder {0}", folderPath));
                foreach (var extension in extensions)
                {
                    var files = Directory.EnumerateFiles(folderPath, extension, SearchOption.AllDirectories);

                    foreach (var filePath in files)
                    {
                        AddDocument(filePath);
                    }
                }            
            });

            ShowMessageInStatusBar(string.Format("Finished scanning folder {0}", folderPath));
            
            //After folder scan, resort data
            SortByColumn(ApplicationSettings.SerializedSetings.SortColumnName,
                _LastSortings[ApplicationSettings.SerializedSetings.SortColumnName]);
        }

        private bool IsDocumentAdded(string filePath)
        {
            return DocumentsDic.ContainsKey(filePath);
        }

        private void AddDocument(string filePath)
        {
            AddDocument(ConvertToDto(filePath));
        }

        private void AddDocument(Document document)
        {
            Dispatcher.Invoke(() =>
            {
                if (!IsDocumentAdded(document.FilePath)
                    && !document.FileName.StartsWith("~$") //ignore temp files
                    )
                {
                    //Add
                    Documents.Add(document);
                }
            });
        }

        /// <summary>
        /// Reload from service
        /// </summary>
        /// <param name="filePath"></param>
        private void ReloadDocument(string filePath)
        {
            var file = Service.GetFiles().FirstOrDefault(k => k.FilePath.Equals(filePath, Const.DefaultStringComparison));

            if (file != null)
            {
                var document = Convert(file);

                if (DocumentsDic.ContainsKey(filePath))
                {
                    //Update                    
                    var d = DocumentsDic[filePath];
                    var index = Documents.IndexOf(d);
                    Documents.RemoveAt(index);
                    Documents.Insert(index, document);
                    SelectedDocument = document;
                }
                else
                {
                    AddDocument(document);
                }
            }
        }

        private void RemoveDocument(string filePath)
        {
            if (DocumentsDic.ContainsKey(filePath))
            {
                Documents.Remove(DocumentsDic[filePath]);
            }
        }

        private void RemoveDocument(Document document)
        {
            RemoveDocument(document.FilePath);
        }

        private Document ConvertToDto(string filePath)
        {
            var to = new Document();
            to.FolderPath = Path.GetDirectoryName(filePath);
            to.FilePath = filePath.ToLower();
            to.FileName = Path.GetFileName(filePath);
            to.FileExtension = Path.GetExtension(filePath);

            return to;
        }        


        private void StartWatchers()
        {
            var folders = ApplicationSettings.Folders.ToList();
 
            foreach (var folder in folders)
            {
                string f = folder;

                if (Directory.Exists(f))
                {
                    var watcher = new Watcher(folder);

                    watcher.FileCreated += (sender, args) =>
                    {
                        if (GetFileExtensionsToScan().Contains(Path.GetExtension(args.FullPath)))
                        {
                            AddDocument(args.FullPath);
                        }
                    };

                    watcher.FileRenamed += (sender, args) =>
                    {
                        if (args.ChangeType == WatcherChangeTypes.Renamed)
                        {
                            if (DocumentsDic.ContainsKey(args.OldFullPath))
                            {
                                var oldDoc = DocumentsDic[args.OldFullPath];
                                DocumentsDic.Remove(args.OldFullPath);
                                //Assign new path
                                var newDoc = ConvertToDto(args.FullPath);
                                oldDoc.FilePath = newDoc.FilePath;
                                oldDoc.FolderPath = newDoc.FolderPath;
                                oldDoc.FileName = newDoc.FileName;
                                oldDoc.FileExtension = newDoc.FileExtension;
                                DocumentsDic.Add(oldDoc.FilePath, oldDoc);
                                UpdateDocumentInfoAsync(DocumentsDic[oldDoc.FilePath]);
                            }
                        }
                    };

                    watcher.FileDeleted += (sender, args) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            RemoveDocument(args.FullPath);
                        });
                    };

                    watcher.FileChanged += (sender, args) =>
                    {
                        if (DocumentsDic.ContainsKey(args.FullPath))
                        {
                            UpdateDocumentInfoAsync(DocumentsDic[args.FullPath]);                            
                        }
                    };
                }
            }
        }

        private void SetSortingOnColumn(AGridViewColumn column)
        {
            DataTemplate tmpTemplate;
            if (_LastSortColumn != null && _LastSortColumn != column)
            {
                //Clear sorting style for old column

                tmpTemplate = listView.TryFindResource("HeaderTemplateSort") as DataTemplate;

                if (tmpTemplate != null)
                {
                    _LastSortColumn.HeaderTemplate = tmpTemplate;
                }
            }

            if (column != null)
            {
                var lastSortDirection = _LastSortings[column.SortPropertyName];
                
                switch (lastSortDirection)
                {
                    case ListSortDirection.Ascending:

                        SortByColumn(column.SortPropertyName, ListSortDirection.Descending);
                        _LastSortings[column.SortPropertyName] = ListSortDirection.Descending;
                        tmpTemplate = listView.TryFindResource("HeaderTemplateSortDesc") as DataTemplate;

                        if (tmpTemplate != null)
                        {
                            column.HeaderTemplate = tmpTemplate;
                        }

                        break;
                    case ListSortDirection.Descending:

                        SortByColumn(column.SortPropertyName, ListSortDirection.Ascending);
                        _LastSortings[column.SortPropertyName] = ListSortDirection.Ascending;
                        tmpTemplate = listView.TryFindResource("HeaderTemplateSortAsc") as DataTemplate;

                        if (tmpTemplate != null)
                        {
                            column.HeaderTemplate = tmpTemplate;
                        }

                        break;
                }

                _LastSortColumn = column;
            }
        }

        private void OnColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            var header = e.OriginalSource as GridViewColumnHeader;

            if (header != null)
            {
                SetSortingOnColumn(header.Column as AGridViewColumn);
                e.Handled = true;
            }
        }

        private bool ContaintsString(string source, string parameter)
        {
            if (source != null && source.ToLower().Contains(parameter))
            {
                return true;
            }

            return false;
        }


        private void Filter()
        {
            var filterText = filterTextBox.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            
            if (!string.IsNullOrEmpty(filterText))
            {
                var parameters = filterText.Split(' ', ',', ';').Where(k => !string.IsNullOrWhiteSpace(k)).ToList();

                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i] = parameters[i].ToLower();                    
                }

                view.Filter = p =>
                {
                    var document = p as IDocument;

                    foreach (var parameter in parameters)
                    {                       
                        if (ContaintsString(document.FileName, parameter))
                        {
                            continue;
                        }

                        if (ContaintsString(document.FolderPath, parameter))
                        {
                            continue;
                        }

                        if (ContaintsString(document.DateString, parameter))
                        {
                            continue;
                        }

                        if (ContaintsString(document.Description, parameter))
                        {
                            continue;
                        }

                        if (ContaintsString(document.Project, parameter))
                        {
                            continue;
                        }

                        if (ContaintsString(document.Owner, parameter))
                        {
                            continue;
                        }
                        
                        if (ContaintsString(document.KeywordsString, parameter)) 
                        {
                            continue;
                        }

                        return false;
                    }

                    return true;
                };

            }
            else
            {
                view.Filter = null;
            }
        }

        private void ShowFileNotFoundDialog(string filePath)
        {
            new MessageBoxDialog(Constants.Error, string.Format(Constants.FileNotFound, filePath)).Show();
        }

        private void ViewDocument(string filePath)
        {
            if (!File.Exists(filePath))
            {
                ShowFileNotFoundDialog(filePath);                
                return;
            }

            using (var process = new Process())
            {
                process.StartInfo.FileName = filePath;

                process.Start();
            }
        }

        private void searchResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewDocument(string.Format(SelectedDocument.FilePath));
        }


        private void filterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
            UpdateDocumentsCountInStatusBar();
        }

        private void filterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                //Clean
                ClearFilterCommand.Execute(null);
                filterTextBox.Focus();
            }
        }

        /// <summary>
        /// Shows the files count.
        /// </summary>
        private void UpdateDocumentsCountInStatusBar()
        {
            ShownDocumentsCount = listView.Items.Count;
            DocumentsTotalCount = Documents.Count;
        }

        private List<Document> GetSelectedDocuments()
        {
            var documents = new List<Document>();

            if (listView != null && listView.SelectedItems != null)
            {
                foreach (var selectedItem in listView.SelectedItems)
                {
                    documents.Add((Document)selectedItem);
                }
            }

            if (!documents.Any() && SelectedDocument != null)
            {
                documents.Add(SelectedDocument);
            }

            return documents;
        }

        private void PrintDocument(string filePath)
        {
            //http:social.msdn.microsoft.com/forums/en-US/csharpgeneral/thread/8ce3dd94-ff2c-4c3c-ba85-f67b9dd2f2e8/
            var printJob = new Process();

            var startInfo = new ProcessStartInfo();

            startInfo.FileName = filePath;
            startInfo.UseShellExecute = true;
            startInfo.Verb = "print";
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            printJob.StartInfo = startInfo;

            printJob.Start();

            //Hide window. NOT WORKING
            if (printJob != null && printJob.MainWindowHandle != null)
            {
                var hWnd = printJob.MainWindowHandle.ToInt32();
                ShowWindow(hWnd, SW_HIDE);
            }
        }

        private void ClearFilter()
        {
            //Clear filters
            filterTextBox.Text = string.Empty;
        }

        private void SortByColumn(string columnName, ListSortDirection direction)
        {
            ApplicationSettings.SerializedSetings.SortColumnName = columnName;
            ApplicationSettings.SerializedSetings.IsSortedAsc = direction == ListSortDirection.Ascending;

            if (listView.ItemsSource != null)
            {
                var view = CollectionViewSource.GetDefaultView(listView.ItemsSource);

                if (view.SortDescriptions != null)
                {
                    view.SortDescriptions.Clear();

                    var sorts = view.SortDescriptions;
                    sorts.Add(new SortDescription(columnName, direction));
                }

                listView.Items.Refresh();
            }
        }

        #endregion

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            IsInProgress = true;

            LoadSavedDocuments();
            StartFullScan();
            StartWatchers();

            IsInProgress = false;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            GridView.SaveSettings();
            Logging.Debug("Application closed");  
        }

        private void ListView_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var documents = GetSelectedDocuments();

                foreach (var doc in documents)
                {
                    ViewDocument(doc.FilePath);
                }
            }
            else if (e.Key == Key.F2)
            {
                if (EditCanExecute())
                {
                    EditExecute();
                }
            }

            filterTextBox_KeyUp(sender, e);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ApplicationSettings.SerializedSetings.IsWindowMaximized = WindowState == WindowState.Maximized;
            ApplicationSettings.SerializedSetings.WindowTop = Top;
            ApplicationSettings.SerializedSetings.WindowLeft = Left;
        }

        void ShortcutButton_Click(object sender, RoutedEventArgs e)
        {
            SortByColumn(ApplicationSettings.SerializedSetings.SortColumnName,
                _LastSortings[ApplicationSettings.SerializedSetings.SortColumnName]);
            filterTextBox.Text = ((AButton)sender).FilterText;
            filterTextBox.Focus();
        }

        private void MenuFilterTextChange_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            var b = ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget as AButton;
            b.FilterText = filterTextBox.Text;
            var t = b.FilterText;
            switch (b.Number)
            {
                case 1:
                    ApplicationSettings.SerializedSetings.Filter01Text = t;
                    break;
                case 2:
                    ApplicationSettings.SerializedSetings.Filter02Text = t;
                    break;
                case 3:
                    ApplicationSettings.SerializedSetings.Filter03Text = t;
                    break;
                case 4:
                    ApplicationSettings.SerializedSetings.Filter04Text = t;
                    break;
                case 5:
                    ApplicationSettings.SerializedSetings.Filter05Text = t;
                    break;
                case 6:
                    ApplicationSettings.SerializedSetings.Filter06Text = t;
                    break;
                case 7:
                    ApplicationSettings.SerializedSetings.Filter07Text = t;
                    break;
                case 8:
                    ApplicationSettings.SerializedSetings.Filter08Text = t;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MenuFilterNameChange_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            var b = ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget as AButton;
            
            Popup popup = new Popup();            
            popup.PlacementTarget = b;
            popup.Placement = PlacementMode.Top;
            popup.Width = 200;
            var textBox = new TextBox();
            textBox.ToolTip = "Press Enter to close";
            textBox.Text = b.FilterName;
            popup.Child = textBox;
            popup.IsOpen = true;
            textBox.Focus();
            popup.KeyUp += FilterNamePopup_KeyUp;
        }

        void FilterNamePopup_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var popup = sender as Popup;
                popup.IsOpen = false;
                var textBox = popup.Child as TextBox;
                var b = popup.PlacementTarget as AButton;
                b.FilterName = textBox.Text;

                var t = b.FilterName;
                switch (b.Number)
                {
                    case 1:
                        ApplicationSettings.SerializedSetings.Filter01Name = t;
                        break;
                    case 2:
                        ApplicationSettings.SerializedSetings.Filter02Name = t;
                        break;
                    case 3:
                        ApplicationSettings.SerializedSetings.Filter03Name = t;
                        break;
                    case 4:
                        ApplicationSettings.SerializedSetings.Filter04Name = t;
                        break;
                    case 5:
                        ApplicationSettings.SerializedSetings.Filter05Name = t;
                        break;
                    case 6:
                        ApplicationSettings.SerializedSetings.Filter06Name = t;
                        break;
                    case 7:
                        ApplicationSettings.SerializedSetings.Filter07Name = t;
                        break;
                    case 8:
                        ApplicationSettings.SerializedSetings.Filter08Name = t;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        void MenuIconChange_Click(object sender, RoutedEventArgs e)
        {
            var b = ((ContextMenu)(sender as MenuItem).Parent).PlacementTarget as AButton;                

            var dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.ico, *.icon)|*.ico;*.icon";
            var result = dialog.ShowDialog();            
           
            if (result == System.Windows.Forms.DialogResult.OK &&
                !string.IsNullOrWhiteSpace(dialog.FileName))
            {                
                b.IconPath = dialog.FileName;

                switch (b.Number)
                {
                    case 1:
                        ApplicationSettings.SerializedSetings.Filter01Icon = b.IconPath;
                        break;
                    case 2:
                        ApplicationSettings.SerializedSetings.Filter02Icon = b.IconPath;
                        break;
                    case 3:
                        ApplicationSettings.SerializedSetings.Filter03Icon = b.IconPath;
                        break;
                    case 4:
                        ApplicationSettings.SerializedSetings.Filter04Icon = b.IconPath;
                        break;
                    case 5:
                        ApplicationSettings.SerializedSetings.Filter05Icon = b.IconPath;
                        break;
                    case 6:
                        ApplicationSettings.SerializedSetings.Filter06Icon = b.IconPath;
                        break;
                    case 7:
                        ApplicationSettings.SerializedSetings.Filter07Icon = b.IconPath;
                        break;
                    case 8:
                        ApplicationSettings.SerializedSetings.Filter08Icon = b.IconPath;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #region Drag&Drop

        private Point _Start;

        private void FileView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _Start = e.GetPosition(null);
        }

        private void FileView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mpos = e.GetPosition(null);
            Vector diff = _Start - mpos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (this.listView.SelectedItems.Count == 0)
                    return;

                // right about here you get the file urls of the selected items.  
                // should be quite easy, if not, ask.  
                string[] files = GetSelectedDocuments().Select(k => k.FilePath).ToArray();

                string dataFormat = DataFormats.FileDrop;
                DataObject dataObject = new DataObject(dataFormat, files);
                DragDrop.DoDragDrop(this.listView, dataObject, DragDropEffects.Copy);
            }
        }

        #endregion
    }
}



