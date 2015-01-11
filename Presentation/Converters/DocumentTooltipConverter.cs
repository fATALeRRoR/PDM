using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using PDM.Common.Interfaces;

namespace PDM.Client.Converters
{
    [ValueConversion(typeof(IDocument), typeof(string))]
    public class DocumentTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Performance can be improved by removing converter and making property on Document
            var doc = value as IDocument;

            var builder = new StringBuilder();
            builder.AppendLine(string.Format("File name: {0}", doc.FileName));
            builder.AppendLine(string.Format("File path: {0}", doc.FilePath));
            builder.AppendLine(string.Format("Date: {0}", doc.DateString));
            builder.AppendLine(string.Format("Last access time: {0}", doc.LastAccessTimeString));
            builder.AppendLine(string.Format("Last write time: {0}", doc.LastWriteTimeString));
            builder.AppendLine(string.Format("Is deleted: {0}", doc.IsDeleted));
            builder.AppendLine(string.Format("Keywords: {0}", doc.KeywordsString));

            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}