using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using PDM.BusinessLogic;
using PDM.BusinessLogic.Service;
using PDM.Client.Common;
using PDM.Client.Controls;
using PDM.Client.Dto;
using PDM.Client.Helpers;

namespace PDM.Client.Windows
{
    /// <summary>
    /// Interaction logic for EditDocumentWindow.xaml
    /// </summary>
    public partial class EditDocumentWindow
    {
        public EditDocumentWindow()
        {
            InitializeComponent();

            //Set data context
            DataContext = this;

            //Commands
            AddKeywordCommand = new Command(AddKeywordExecute, AddKeywordCanExecute);
            RemoveKeywordCommand = new Command(RemoveKeyword, RemoveKeywordCanExecute);
        }

        #region Commands

        public Command AddKeywordCommand { get; set; }
        public Command RemoveKeywordCommand { get; set; }

        private void AddKeywordExecute()
        {
            Document.Keywords.Add(Keyword);
            Keyword = null;
            KeywordTextBox.Focus();
        }

        private bool AddKeywordCanExecute()
        {
            return !string.IsNullOrWhiteSpace(Keyword);
        }

        private void RemoveKeyword()
        {
            Document.Keywords.Remove(SelectedKeyword);
        }

        private bool RemoveKeywordCanExecute()
        {
            return !string.IsNullOrWhiteSpace(SelectedKeyword);
        }

        #endregion


        private Document _Document;
        public Document Document
        {
            get { return _Document; }
            set
            {
                _Document = value;
                OnPropertyChanged();

                if (_Document != null)
                {
                    AutoCompleteTextBox.Text = _Document.Project;
                }
                else
                {
                    AutoCompleteTextBox.Text = null;
                }
            }
        }

        public DateTime FileDate { get; set; }

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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //Workaround for autocomplete
            Document.Project = AutoCompleteTextBox.Text;
            DeletedFileHelper.RemoveDeletedKeyword(Document);

            if (File.Exists(Document.FilePath))
            {
                DeletedFileHelper.RemoveDeletedKeyword(Document);
            }

            IBusinessService service = new BusinessService();            
            service.CreateOrUpdateFileAsync(Document);

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

        private void TodayDateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Document.Date = DateTime.Today;
        }

        private void FileDateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Document.Date = FileDate;
        }

        public void LoadProjectsForAutoComplete(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                var entry = new AutoCompleteEntry(item, item);
                AutoCompleteTextBox.AddItem(entry);
            }
        }
    }
}
