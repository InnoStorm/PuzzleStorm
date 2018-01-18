using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.SubDTOs;

namespace DTOLibrary.Responses
{
    public class GetAllRoomsResponse : Response
    {
        public List<RoomInfo> List { get; set; }
    }
}
