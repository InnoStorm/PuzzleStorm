using System.Windows;

namespace Client {
    class WindowViewModel : BaseViewModel {

        #region Private

        private Window mWindow;

        #endregion

        #region Properties

        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.GameOverPage;

        #endregion

        #region Constructors

        public WindowViewModel(Window window)
        {
            this.mWindow = window;
        }

        #endregion
    }
}
