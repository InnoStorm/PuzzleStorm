using System.Collections.Generic;

namespace Client {

    public sealed class ListRooms {

        private static ListRooms instance = null;

        public List<RoomsPropsViewModel> RoomsItemsList { get; set; }

        private ListRooms() {
            RoomsItemsList = new List<RoomsPropsViewModel>();
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
