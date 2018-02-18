using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Helpers.Enums
{
    public enum WindowTransition
    {
        LoginToHome, //a
        HomeToCreateRoom, //b
        CreateRoomToLobbyOwner, //c
        LobbyOwnerToGameplay, //d
        LobbyOwnerToHome, //e
        HomeToLobbyJoiner, //f
        LobbyJoinerToHome, //g
        LobbyJoinerToGameplay, //h i
        CreateRoomToHome, //j

        HomeToHome, //three dots thing
        
        HomeToLogin
    }
}