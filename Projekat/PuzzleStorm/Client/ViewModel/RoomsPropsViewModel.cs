using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Client {

    public class RoomsPropsViewModel {

        public RoomsPropsViewModel()
        {
            JoinCommand = new RelayCommand(async () => await JoinInRoomAsync());
        }

        #region Properties

        public int RoomId { get; set; }

        public string Name { get; set; }

        public string By { get; set; }

        public string Difficulty { get; set; }

        public string MaxPlayers { get; set; }

        public string Rounds { get; set; }

        #endregion

        #region Commands

        public ICommand JoinCommand { get; set; }

        #endregion

        #region Metods

        private async Task JoinInRoomAsync() {
            var sampleMessageDialog = new SampleMessageDialog {
                Message = { Text = "Blabla" }
            };

            await DialogHost.Show(sampleMessageDialog);
        }

        #endregion
    }
}
