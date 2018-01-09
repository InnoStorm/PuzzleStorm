using System.Windows;
using System.Windows.Input;

namespace Client {

    /// <summary>
    /// ViewModel za main stranicu
    /// </summary>
    public class MainPageViewModel : BaseViewModel {

        #region Properties

        //Listu za 3 main sobe


        #endregion

        #region Commands

        /// <summary>
        /// Komanda koja vodi do sve sobe klikom na tri tacke
        /// </summary>
        public ICommand TriTackeCommand { get; set; }

        /// <summary>
        /// Komadna za log out iz main stranice
        /// </summary>
        public ICommand LogOutCommand { get; set; }

        /// <summary>
        /// Komanda koja otvara prozor za kreiranje nove sobe
        /// </summary>
        public ICommand CreateNewRoomCommand { get; set; }

        #endregion

        #region Constructors

        public MainPageViewModel()
        {
            TriTackeCommand = new RelayCommand(() => TriTackeButton());
            LogOutCommand = new RelayCommand(() => LogOutButton());

            CreateNewRoomCommand = new RelayCommand(() => CreateNewRoom());
        }

        #endregion

        #region Metode

        /// <summary>
        /// F-ja kad klikne dugme tri tacke
        /// </summary>
        public void TriTackeButton()
        {
            
        }

        /// <summary>
        /// Dugme za logout
        /// </summary>
        public void LogOutButton()
        {

            //neko brisanje pre log outa ?? 

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LoginPage();;
        }

        /// <summary>
        /// F-ja za novu sobu
        /// </summary>
        public void CreateNewRoom()
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new CreateRoomPage();;
        }

        #endregion

    }
}
