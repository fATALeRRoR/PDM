using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PDM.Client.Controls
{
    public class AButton : Button
    {
        private readonly Image _Image = new Image();
        private readonly Label _TextBlock = new Label();

        public AButton()
        {
            MinWidth = 50;
            VerticalAlignment = VerticalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Center;

            _TextBlock.VerticalAlignment = VerticalAlignment.Stretch;
            _TextBlock.VerticalContentAlignment = VerticalAlignment.Center;

            var stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;   
            stackPanel.Children.Add(_Image);
            stackPanel.Children.Add(_TextBlock);
            Content = stackPanel;
        }

        private int _Number;
        public int Number
        {
            get { return _Number; }
            set
            {
                _Number = value;
                if (_Number > 0)
                {
                    Margin = new Thickness(5, 0, 0, 0);
                }
            }
        }

        private string _FilterName;
        public string FilterName
        {
            get { return _FilterName; }
            set
            {
                _FilterName = value;
                _TextBlock.Content = string.Format("_{0} {1}", Number, _FilterName);
            }
        }

        private string _FilterText;
        public string FilterText
        {
            get { return _FilterText; }
            set
            {
                _FilterText = value;                
                ToolTip = string.Format("ALT + {0} : {1}", Number, _FilterText);
            }
        }

        private string _IconPath;
        public string IconPath
        {
            get { return _IconPath; }
            set
            {
                _IconPath = value;
                if (File.Exists(_IconPath))
                {
                    _Image.Source = new BitmapImage(new Uri(_IconPath));
                }
            }
        }
    }
}
