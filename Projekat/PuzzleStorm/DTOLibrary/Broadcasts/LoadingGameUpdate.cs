using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.SubDTOs;

namespace DTOLibrary.Broadcasts
{
    public class LoadingGameUpdate : BroadcastMessage
    {
        public List<Player> LoadedPlayers { get; set; }
        public bool AreAllLoaded { get; set; }
    }
}
