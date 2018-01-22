using System.Collections.Generic;
using System.Windows;

namespace Client {

    public sealed class ListRooms {

        private static ListRooms instance = null;

        public List<RoomsPropsViewModel> RoomsItemsList { get; set; }

        private ListRooms()
        {
            RoomsItemsList = new List<RoomsPropsViewModel>()
            {
                new RoomsPropsViewModel()
                {
                    By = "Dusan",
                    Visibility = Visibility.Visible,
                    Difficulty = "25",
                    Rounds = "5",
                    MaxPlayers = "5",
                    Name = "Room #23",
                    RoomId = 23,
                    Locked = false
                }
            };
        }

        public static ListRooms Instance {
            get {
                if (instance == null) {
                    instance = new ListRooms();
                }

                return instance;
            }
        }
    }
}
