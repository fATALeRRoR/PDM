using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PDM.BusinessLogic
{
    public class WindowBase : Window, INotifyPropertyChanged
    {
        public WindowBase()
        {
            //Default settings 
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;
            Icon = GetIcon();
            MinWidth = 300;
            MinHeight = 150;            
        }

        private System.Windows.Media.ImageSource GetIcon()
        {
            IconBitmapDecoder ibd = new IconBitmapDecoder(
                new Uri(@"pack://application:,,/Images/Icon.ico", UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.None,
                BitmapCacheOption.Default);

            return ibd.Frames[0];
        }

        public override string ToString()
        {
            return Title;
        }

        #region INotifyPropertyChanged

        protected void OnPropertyChanged([CallerMemberName]string status = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(status));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}