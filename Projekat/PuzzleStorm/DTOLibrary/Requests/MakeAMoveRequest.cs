using DTOLibrary.SubDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Requests
{
    public class MakeAMoveRequest : PostLoginRequest
    {
        public int RoomId { get; set; }
        public Move MoveToPlay;
    }
}
