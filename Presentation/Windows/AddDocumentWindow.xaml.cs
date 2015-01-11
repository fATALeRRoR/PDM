using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using PDM.BusinessLogic;
using PDM.BusinessLogic.Service;
using PDM.Client.Common;
using PDM.Client.Controls;
using PDM.Client.Dto;
using PDM.Client.Extensions;
using PDM.Client.Helpers;
using PDM.Client.Settings;

namespace PDM.Client.Windows
{
    /// <summary>
    /// Interaction logic for AddDocumentWindow.xaml
    /// </summary>
    public partial class AddDocumentWindow 
    {
        public AddDocumentWindow()
        {
            InitializeComponent();

            //Set data context
            DataContext = this;

            LoadTemplates();

            //Commands
            AddKeywordCommand = new Command(AddKeywordExecute, AddKeywordCanExecute);
            RemoveKeywordCommand = new Command(RemoveKeyword, RemoveKeywordCanExecute);
        }

        private string _OriginalFilePath;
        public string OriginalFilePath
        {
            get { return _OriginalFilePath; }
            set
            {
                _OriginalFilePath = value;
                OnPropertyChanged();
            }
        }

        private string _Folder;
        public string Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
                OnPropertyChanged();
            }
        }

        private string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                OnPropertyChanged();
            }
        }

        private DateTime _Date = DateTime.Today;
        public DateTime Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }

        public DateTime FileDate { get; set; }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                OnPropertyChanged();
            }
        }

        public string Project
        {
            get { return AutoCompleteTextBox.Text; }
            set
            {
                AutoCompleteTextBox.Text = value;
                OnPropertyChanged();
            }
        }

        private string _DocumentOwner = "Pierre"; 
        public string DocumentOwner
        {
            get { return _DocumentOwner; }
            set
            {
                _DocumentOwner = value;
                OnPropertyChanged();
            }
        }

        private bool _IsCopyFromFileSelected;
        public bool IsCopyFromFileSelected
        {
            get { return _IsCopyFromFileSelected; }
            set
            {
                _IsCopyFromFileSelected = value;
                OnPropertyChanged();
                _IsCopyFromTemplateSelected = !_IsCopyFromFileSelected;
            }
        }

        private bool _IsCopyFromTemplateSelected;
        public bool IsCopyFromTemplateSelected
        {
            get { return _IsCopyFromTemplateSelected; }
            set
            {
                _IsCopyFromTemplateSelected = value;
                OnPropertyChanged();
                _IsCopyFromFileSelected = !_IsCopyFromTemplateSelected;
                _SelectedTemplate = Templates.FirstOrDefault();
                OnPropertyChanged("SelectedTemplate");
            }
        }

        #region Keywords

        public Command AddKeywordCommand { get; set; }
        public Command RemoveKeywordCommand { get; set; }

        private void AddKeywordExecute()
        {
            Keywords.Add(Keyword);
            Keyword = null;
            KeywordTextBox.Focus();
        }

        private bool AddKeywordCanExecute()
        {
            return !string.IsNullOrWhiteSpace(Keyword);
        }

        private void RemoveKeyword()
        {
            Keywords.Remove(SelectedKeyword);
        }

        private bool RemoveKeywordCanExecute()
        {
            return !string.IsNullOrWhiteSpace(SelectedKeyword);
        }

        #endregion


        private string _Keyword;
        public string Keyword
        {
            get { return _Keyword; }
            set
            {
                _Keyword = value;
                OnPropertyChanged();
                AddKeywordCommand.RaiseCanExecuteChanged();
            }
        }

        private string _SelectedKeyword;
        public string SelectedKeyword
        {
            get { return _SelectedKeyword; }
            set
            {
                _SelectedKeyword = value;
                OnPropertyChanged();
                RemoveKeywordCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<string> _Keywords = new ObservableCollection<string>();
        public ObservableCollection<string> Keywords
        {
            get { return _Keywords; }
            set
            {
                _Keywords = value;
                OnPropertyChanged();
            }
        }

        public void LoadProjectsForAutoComplete(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                var entry = new AutoCompleteEntry(item, item);
                AutoCompleteTextBox.AddItem(entry);
            }
        }

        private void LoadTemplates()
        {
            Templates = ApplicationSettings.Templates.ToList();
        }

        private List<string> _Templates;
        public List<string> Templates
        {
            get { return _Templates; }
            set
            {
                _Templates = value;
                OnPropertyChanged();
            }
        }

        private string _SelectedTemplate;
        public string SelectedTemplate
        {
            get { return _SelectedTemplate; }
            set
            {
                _SelectedTemplate = value;
                OnPropertyChanged();

                if (!string.IsNullOrWhiteSpace(_SelectedTemplate))
                {
                    IsCopyFromTemplateSelected = true;
                }
            }
        }

        public Document SavedDocument { get; set; }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var oldFilePath = OriginalFilePath;

            if (IsCopyFromTemplateSelected)
            {
                oldFilePath = SelectedTemplate;
            }

            if (!File.Exists(oldFilePath))
            {
                new MessageBoxDialog(Constants.Error, string.Format(Constants.FileNotFound, oldFilePath)).Show();                
                return;
            }

            var newFilePath = Path.Combine(Folder, FileName);

            if (File.Exists(newFilePath))
            {
                var message = string.Format("File exists ({0}). Overwrite?", newFilePath);
                var result = new MessageBoxDialog("Overwrite", message, MessageBoxButton.YesNo).Show();

                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    File.Copy(oldFilePath, newFilePath, true);
                }
                else
                {
                    return;
                }
            }
            else
            {
                File.Copy(oldFilePath, newFilePath);
            }            

            SavedDocument = new Document();
            SavedDocument.FilePath = newFilePath;
            SavedDocument.Date = Date;
            SavedDocument.Description = Description;
            SavedDocument.Project = Project;
            SavedDocument.Owner = DocumentOwner;
            SavedDocument.Keywords = Keywords;

            DeletedFileHelper.RemoveDeletedKeyword(SavedDocument);

            if (File.Exists(SavedDocument.FilePath))
            {
                DeletedFileHelper.RemoveDeletedKeyword(SavedDocument);
            }

            IBusinessService service = new BusinessService();
            service.CreateOrUpdateFileAsync(SavedDocument);

            DialogResult = true; 
            Close();
            e.Handled = true;            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
            e.Handled = true;
        }

        private void KeywordTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && AddKeywordCanExecute())
            {
                AddKeywordExecute();
            }
        }

        private void FileNameButton_OnClick(object sender, RoutedEventArgs e)
        {
            FileName = IncreaseName(FileName);
        }

        /*
        '+1 functionality does not work correctly in case of leading zeros or in case no number exists yet:
        - ABC.DOC: +1 does not work
        - CDEFv1.4.DOC: +1 leads to CDEFv2.4.DOC. Best is to always +1 the digit most to the right just before the file extension
        - RPT0021.DOC: +1 leads to RPT22.DOC. I would like to keep the leading zero's         
         */

        public static string IncreaseName(string fileName)
        {
            Regex regex;

            if (fileName.IndexOf(".") != fileName.LastIndexOf("."))
            {
                regex = new Regex(@"[0-9]{1,10}[.,][0-9]{0,10}");                
            }
            else
            {
                regex = new Regex(@"[0-9]{1,10}");    
            }

            Match match = regex.Match(fileName);

            if (match.Success)
            {
                //var stringNumber = match.Groups[match.Groups.Count - 1].Value.Trim('.');
                var stringNumber = match.Value.Trim('.');

                var index = stringNumber.LastIndexOf('.');

                if (index > 0)
                {
                    stringNumber = stringNumber.Substring(index + 1);
                    index = match.Index + index + 1;
                }
                else
                {
                    index = match.Index;
                }

                int number;

                if (int.TryParse(stringNumber, out number))
                {

                    fileName = fileName.Remove(index, stringNumber.Length);
                    fileName = fileName.Insert(index, (number + 1).ToString().PadLeft(stringNumber.Length, '0'));
                }
            }
            else
            {
                //ABC.DOC: +1 does not work
                fileName = fileName.Insert(fileName.LastIndexOf("."), "1");
            }

            return fileName;
        }
        
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            Date = DateTime.Today;
            Description = null;
            Project = null;
            DocumentOwner = null;
            Keywords.Clear();
        }

        private void TodayDateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Date = DateTime.Today;
        }

        private void FileDateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Date = FileDate;
        }
    }
}
