using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormCommonData.Enums;
using DTOLibrary.Requests;
using DTOLibrary.SubDTOs;
using EasyNetQ;

namespace DTOLibrary.Broadcasts
{
    [Queue(queueName: "RoomPlayerUpdateQueue", ExchangeName = "InRoomUpdatesExchange")]
    public class RoomPlayerUpdate : BroadcastMessage
    {
        public RoomPlayerUpdateType UpdateType { get; set; }
        public int PlayerId { get; set; }
        public int RoomId { get; set; }
    }
}
