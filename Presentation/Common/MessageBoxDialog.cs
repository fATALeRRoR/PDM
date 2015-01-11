using System.Windows;

namespace PDM.Client.Common
{
    public class MessageBoxDialog
    {
        private readonly string _Header;
        private readonly string _Message;
        private readonly MessageBoxButton _Buttons = MessageBoxButton.OK;

        public MessageBoxDialog(string header, string message)
        {
            _Header = header;
            _Message = message;            
        }

        public MessageBoxDialog(string header, string message, MessageBoxButton buttons)
        {
            _Header = header;
            _Message = message;
            _Buttons = buttons;
        }

        public MessageBoxResult Show()
        {
            return MessageBox.Show(_Message, _Header, _Buttons);
        }
    }
}
