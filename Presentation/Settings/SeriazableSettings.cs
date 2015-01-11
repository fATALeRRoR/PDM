using System;
using PDM.Client.Common;

namespace PDM.Client.Settings
{
    /// <summary>
    /// For storing sort, column width, application size, position and maximized state
    /// </summary>
    [Serializable]
    public class SeriazableSettings
    {
        public SeriazableSettings()
        {
            SortColumnName = Constants.DateProperty;

            FileNameExtensionWidth = 30;
            FileNameColumnWidth = 130;
            FolderPathColumnWidth = 150;
            DateColumnWidth = 100;
            DescriptionColumnWidth = 140;
            ProjectColumnWidth = 140;
            OwnerColumnWidth = 140;
            KeywordsColumnWidth = 200;


            IsWindowMaximized = true;
        }

        public string SortColumnName { get; set; }
        public bool IsSortedAsc { get; set; }

        public double FileNameExtensionWidth { get; set; }
        public double FileNameColumnWidth { get; set; }
        public double FolderPathColumnWidth { get; set; }
        public double DateColumnWidth { get; set; }
        public double DescriptionColumnWidth { get; set; }
        public double ProjectColumnWidth { get; set; }
        public double OwnerColumnWidth { get; set; }
        public double KeywordsColumnWidth { get; set; }

        public bool IsWindowMaximized { get; set; }
        public double WindowTop { get; set; }
        public double WindowLeft { get; set; }

        public string Filter01Name { get; set; }
        public string Filter02Name { get; set; }
        public string Filter03Name { get; set; }
        public string Filter04Name { get; set; }
        public string Filter05Name { get; set; }
        public string Filter06Name { get; set; }
        public string Filter07Name { get; set; }
        public string Filter08Name { get; set; }

        public string Filter01Text { get; set; }
        public string Filter02Text { get; set; }
        public string Filter03Text { get; set; }
        public string Filter04Text { get; set; }
        public string Filter05Text { get; set; }
        public string Filter06Text { get; set; }
        public string Filter07Text { get; set; }
        public string Filter08Text { get; set; }

        public string Filter01Icon { get; set; }
        public string Filter02Icon { get; set; }
        public string Filter03Icon { get; set; }
        public string Filter04Icon { get; set; }
        public string Filter05Icon { get; set; }
        public string Filter06Icon { get; set; }
        public string Filter07Icon { get; set; }
        public string Filter08Icon { get; set; }        
    }
}
