using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client {

    public sealed class ListRooms {

        private static ListRooms instance = null;

        //public List<RoomsPropsViewModel> RoomsItemsList { get; set; }
        public ObservableCollection<RoomsPropsViewModel> RoomsItemsList { get; set; }

        private ListRooms()
        {
            //RoomsItemsList = new List<RoomsPropsViewModel>();
            RoomsItemsList = new ObservableCollection<RoomsPropsViewModel>();
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
