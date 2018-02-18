using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Requests
{
    public class MakeAMoveRequest : PostLoginRequest
    {
        public int RoomId;
        public int SelectedPartNumber;
        public int TablePlaceNumber;
    }
}
