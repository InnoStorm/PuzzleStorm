using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Client {

    public class RoomsPropsViewModel {

        #region Constructors

        public RoomsPropsViewModel() {
            JoinCommand = new RelayCommandWithParameter(async (paramater) => await JoinInRoomAsync(paramater));
        }

        #endregion

        #region Properties

        public int RoomId { get; set; }

        public string Name { get; set; }

        public string By { get; set; }

        public string Difficulty { get; set; }

        public string MaxPlayers { get; set; }

        public string Rounds { get; set; }

        public bool Locked { get; set; }

        public Visibility Visibility { get; set; }

        #endregion

        #region Commands

        public ICommand JoinCommand { get; set; }

        #endregion

        #region Metods

        public async Task JoinInRoomAsync(object parametar)
        {
            var dialog = new SimpleTextBoxDialog()
            {
                Message = { Text = "ENTER PASSWORD" },
                DialogButton = { Content = "JOIN" }
            };

            await DialogHost.Show(dialog, delegate(object sender, DialogClosingEventArgs args)
            {
                if (args.Parameter != null && !string.IsNullOrWhiteSpace((string) args.Parameter))
                {
                    
                }
            });
        }

        #endregion
    }
}
