using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Responses
{
    public class StartGameResponse : Response
    {
        public int GameId { get; set; }
        public string CommunicationKey { get; set; }
    }
}
