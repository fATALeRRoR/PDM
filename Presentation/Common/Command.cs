using System;
using System.Windows.Input;

namespace PDM.Client.Common
{
    public class Command : ICommand 
    {
        private readonly Action _Execute;
        private readonly Func<bool> _CanExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="executeMethod">The execute method.</param>
        public Command(Action executeMethod) : this(executeMethod, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="executeMethod">The execute method.</param>
        /// <param name="canExecuteMethod">The can execute method.</param>
        public Command(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _Execute = executeMethod;
            _CanExecute = canExecuteMethod;
        }

        void CanExecuteChangedHandler(object sender, EventArgs e)
        {
            var canExecuteChanged = CanExecuteChanged;

            if (canExecuteChanged != null)
            {
                canExecuteChanged(sender, e);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChangedHandler(this, EventArgs.Empty);
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            var result = true;

            if (_CanExecute != null)
            {
                result = _CanExecute();
            }

            return result;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_Execute != null)
            {
                _Execute();
            }
        }

        #endregion
    }
}
