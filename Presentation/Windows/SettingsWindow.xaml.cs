using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using PDM.Client.Helpers;
using PDM.Client.Settings;

namespace PDM.Client.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            //Set data context
            DataContext = this;

            //Set sorting
            SortFolders();
            SortFileTypes();
            SortTemplates();            
        }        

        #region Event methods

        private void WindowBase_Closed(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void addFolderButton_Click(object sender, RoutedEventArgs e)
        {
            AddFolder();
            e.Handled = true;
        }

        private void removeFolderButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveFolder();
            e.Handled = true;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

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

        #endregion


        #region Private


        /// <summary>
        /// Adds the folder.
        /// </summary>
        private void AddFolder()
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = @"Select a directory to include.";
            dialog.ShowNewFolderButton = true;            

            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK &&
                !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {

                if (IsFolderUnique(dialog.SelectedPath))
                {
                    ApplicationSettings.Folders.Add(dialog.SelectedPath);

                    SortFolders();
                }
            }
        }

        private bool IsFolderUnique(string folder)
        {
            var folders = ApplicationSettings.Folders;

            if (folders != null)
            {
                foreach (var folderOld in folders)
                {
                    //The same folder added twice
                    //New folder is parent of existing folder
                    if (folderOld.ToLower().Contains(folder))
                    {
                        return false;
                    }


                    //New folder is child of existing folder
                    if (folder.ToLower().Contains(folderOld.ToLower()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Sorts folders
        /// </summary>
        private void SortFolders()
        {
            var view = CollectionViewSource.GetDefaultView(foldersListBox.ItemsSource);

            if (view != null && view.SortDescriptions != null)
            {
                view.SortDescriptions.Clear();

                var sorts = view.SortDescriptions;
                sorts.Add(new SortDescription(null, ListSortDirection.Ascending));
                foldersListBox.Items.Refresh();
            }
        }

        private void SortFileTypes()
        {
            var view = CollectionViewSource.GetDefaultView(fileTypesListBox.ItemsSource);

            if (view != null && view.SortDescriptions != null)
            {
                view.SortDescriptions.Clear();

                var sorts = view.SortDescriptions;
                sorts.Add(new SortDescription(null, ListSortDirection.Ascending));
                fileTypesListBox.Items.Refresh();
            }
        }

        /// <summary>
        /// Removes the folder.
        /// </summary>
        private void RemoveFolder()
        {
            if (foldersListBox != null && foldersListBox.SelectedItem != null)
            {
                var selectedItem = foldersListBox.SelectedItem as string;

                //Remove
                if (!string.IsNullOrEmpty(selectedItem))
                {
                    ApplicationSettings.Folders.Remove(selectedItem);

                    SortFolders();
                }
            }
        }


        /// <summary>
        /// Save settings
        /// </summary>
        private void SaveSettings()
        {
            ApplicationSettings.Save();
        }

        #endregion

        private void addFileTypeButton_Click(object sender, RoutedEventArgs e)
        {            
            ApplicationSettings.FileTypes.Add(FileTypeTextBox.Text);
            FileTypeTextBox.Text = null;
            SortFileTypes();
            e.Handled = true;
        }

        private void removeFileTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (fileTypesListBox.SelectedItem != null)
            {
                var selectedItem = fileTypesListBox.SelectedItem as string;           
                ApplicationSettings.FileTypes.Remove(selectedItem);
                SortFileTypes();
                e.Handled = true;
            }
        }

        #region Templates

        private void addTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK &&
                !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                ApplicationSettings.Templates.Add(dialog.FileName);
                SortTemplates();
            }

            e.Handled = true;
        }

        private void removeTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            if (templatesListBox.SelectedItem != null)
            {
                var selectedItem = templatesListBox.SelectedItem as string;
                ApplicationSettings.Templates.Remove(selectedItem);
                SortTemplates();
                e.Handled = true;
            }
        }

        private void SortTemplates()
        {
            var view = CollectionViewSource.GetDefaultView(templatesListBox.ItemsSource);

            if (view != null && view.SortDescriptions != null)
            {
                view.SortDescriptions.Clear();

                var sorts = view.SortDescriptions;
                sorts.Add(new SortDescription(null, ListSortDirection.Ascending));
                templatesListBox.Items.Refresh();
            }            
        }

        #endregion

    }
}