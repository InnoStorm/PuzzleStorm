using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;

namespace DEBUG_PostROman
{
    public static class Utils
    {
        public static LoginResponse LoginData;
        public static CreateRoomResponse CreateRoomData;

        public static BindingList<RoomInfo> AvailableRooms;
    }
}
