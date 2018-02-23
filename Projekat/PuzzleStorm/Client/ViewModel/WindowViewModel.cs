using System;
using System.Windows;
using System.Windows.Input;

namespace Client {
    class WindowViewModel : BaseViewModel {

        #region Private

        private Window mWindow;

        #endregion

        #region Properties

        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;

        #endregion

        #region Command

        public ICommand CloseWindowCommand { get; set;  }

        #endregion

        #region Constructors

        public WindowViewModel(Window window)
        {
            this.mWindow = window;

            CloseWindowCommand = new RelayCommand(DisposeRabbitBus);
        }

        #endregion

        #region Metods

        private void DisposeRabbitBus() {
            //TODO REMOVE
            Player.Instance.Clean();
           
            Communicator.API.Instance.Dispose();
        }

        #endregion
    }
}
