using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.SubDTOs;

namespace DTOLibrary.Broadcasts
{
    public class RoomPlayerUpdate : PostLoginRequest
    {
        public RoomPlayerUpdateType UpdateType { get; set; }
        public Player Player { get; set; }
    }
}
