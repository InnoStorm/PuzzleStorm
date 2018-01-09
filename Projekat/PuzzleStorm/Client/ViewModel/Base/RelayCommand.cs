using System;
using System.Windows.Input;

namespace Client {

    public class RelayCommand: ICommand {

        #region Private

        private Action mAction;

        #endregion

        #region Public

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constuctors

        public RelayCommand(Action action)
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
            mAction();
        }

        #endregion
    }
}
