using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Requests
{
    public class StartGameRequest : PostLoginRequest
    {
        public int RoomId;
    }
}
