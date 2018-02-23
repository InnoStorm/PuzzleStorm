using System;
using System.Diagnostics;
using System.Globalization;

namespace Client {

    /// <summary>
    /// Convertuje ApplicationPage u pravu stranicu
    /// </summary>

    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter> {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            switch ((ApplicationPage) value)
            {
                case ApplicationPage.Login:
                    return new LoginPage();

                case ApplicationPage.CreateAccount:
                    return new CreateAccount();

                case ApplicationPage.GamePage:
                    return new GamePage();

                case ApplicationPage.MainPage:
                    return new MainPage();

                case ApplicationPage.GameOverPage:
                    return new GameOverPage();

                case ApplicationPage.LobbyPage:
                    return new LobbyPage();

                case ApplicationPage.CreateRoom:
                    return new CreateRoomPage();

                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}
