using System.Windows;
using System.Windows.Controls;

namespace PDM.Client.Controls
{
    public class AGridViewColumn : GridViewColumn
    {
        public string SortPropertyName
        {
            get { return (string)GetValue(SortPropertyNameProperty); }
            set { SetValue(SortPropertyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortPropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortPropertyNameProperty =
            DependencyProperty.Register("SortPropertyName", typeof(string), typeof(AGridViewColumn), new UIPropertyMetadata(""));
  

    }
}
