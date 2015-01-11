using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using PDM.Client.Common;
using PDM.Client.Settings;

namespace PDM.Client.Controls
{
    public class AGrdiView : GridView
    {
        public void LoadSettings()
        {
            var settings = ApplicationSettings.SerializedSetings;
            var columns = Columns.Cast<AGridViewColumn>().ToList();

            SetColumnWidth(columns, Constants.FileNameExtensionProperty, settings.FileNameExtensionWidth);
            SetColumnWidth(columns, Constants.FileNameProperty, settings.FileNameColumnWidth);
            SetColumnWidth(columns, Constants.FolderPathProperty, settings.FolderPathColumnWidth);
            SetColumnWidth(columns, Constants.DateProperty, settings.DateColumnWidth);
            SetColumnWidth(columns, Constants.DescriptionProperty, settings.DescriptionColumnWidth);
            SetColumnWidth(columns, Constants.ProjectProperty, settings.ProjectColumnWidth);
            SetColumnWidth(columns, Constants.OwnerProperty, settings.OwnerColumnWidth);
            SetColumnWidth(columns, Constants.KeywordsProperty, settings.KeywordsColumnWidth);
        }

        private void SetColumnWidth(List<AGridViewColumn> columns, string columnName, double width)
        {
            var col = columns.First(k => k.SortPropertyName.Equals(columnName));
            col.Width = width;
        }

        public void SaveSettings()
        {
            var settings = ApplicationSettings.SerializedSetings;
            var columns = Columns.Cast<AGridViewColumn>().ToList();

            foreach (var column in columns)
            {
                if (column.SortPropertyName.Equals(Constants.FileNameExtensionProperty))
                {
                    settings.FileNameExtensionWidth = column.Width;
                }
                else if (column.SortPropertyName.Equals(Constants.FileNameProperty))
                {
                    settings.FileNameColumnWidth = column.Width;
                } 
                else if (column.SortPropertyName.Equals(Constants.FolderPathProperty))
                {
                    settings.FolderPathColumnWidth = column.Width;
                }
                if (column.SortPropertyName.Equals(Constants.DateProperty))
                {
                    settings.DateColumnWidth = column.Width;
                }
                if (column.SortPropertyName.Equals(Constants.DescriptionProperty))
                {
                    settings.DescriptionColumnWidth = column.Width;
                }
                if (column.SortPropertyName.Equals(Constants.ProjectProperty))
                {
                    settings.ProjectColumnWidth = column.Width;
                }
                if (column.SortPropertyName.Equals(Constants.OwnerProperty))
                {
                    settings.OwnerColumnWidth = column.Width;
                }
                if (column.SortPropertyName.Equals(Constants.KeywordsProperty))
                {
                    settings.KeywordsColumnWidth = column.Width;
                }
            }
        }
    }
}
