using System;
using System.Windows.Input;

namespace Client {

    public class RelayCommandWithParameter : ICommand {

        #region Private

        private Action<object> mAction;

        #endregion

        #region Public

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constuctors

        public RelayCommandWithParameter(Action<object> action)
        {
            this.mAction = action;
        }

        #endregion

        #region Commands

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter)
        {
            mAction(parameter);
        }

        #endregion
    }
}
