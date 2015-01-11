using System.Windows.Input;

namespace PDM.BusinessLogic
{
    public class DialogBase : WindowBase
    {
        public DialogBase()
        {
            KeyUp += DialogBase_KeyUp;
            
        }

        void DialogBase_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                e.Handled = true;
            }
        }
    }
}