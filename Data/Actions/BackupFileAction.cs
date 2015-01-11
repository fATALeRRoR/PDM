using System;
using System.Globalization;
using System.IO;
using PDM.BusinessData.Constants;

namespace PDM.BusinessData.Actions
{
    class BackupFileAction
    {
        public void Execute()
        {
            if (!new FileInfo(Files.DataFile).Exists)
            {
                return;
            }

            var fileName = Path.GetFileNameWithoutExtension(Files.DataFile);
             
            var date = DateTime.Now;
            var format = CultureInfo.CurrentCulture.DateTimeFormat;
            var timeSeperator = format.TimeSeparator;
            var dateSeperator = format.DateSeparator;
            const string Seperator = ".";
            var newName = Files.DataFile.Replace(Path.GetFileName(Files.DataFile),
                string.Format("{0}_{1}_{2}.{3}", fileName, date.ToShortDateString().Replace(dateSeperator, Seperator),
                    date.ToLongTimeString().Replace(timeSeperator, Seperator), Path.GetExtension(Files.DataFile)));

            File.Copy(Files.DataFile, newName, true);
        }
    }
}
