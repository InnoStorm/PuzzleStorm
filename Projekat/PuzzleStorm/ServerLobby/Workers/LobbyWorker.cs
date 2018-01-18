using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Core.Domain;
using DataLayer.Persistence;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.Broadcasts;
using Server.Workers;

namespace ServerLobby.Workers
{
    class LobbyWorker : Worker
    {
        public RoomCurrentStateResponse GiveInfoAboutRoom (RoomCurrentStateRequest request)
        {
            try
            {
                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var room = data.Rooms.Find(r => r.Id == request.RoomId).Single();

                    List<DTOLibrary.SubDTOs.Player> players = new List<DTOLibrary.SubDTOs.Player>();

                    foreach (Player p in room.ListOfPlayers)
                        players.Add(new DTOLibrary.SubDTOs.Player()
                        {
                            IsReady = p.IsReady,
                            PlayerId = p.Id,
                            Username = p.UserForPlayer.Username
                        });

                    WorkerLog($"Successfull info about room: { request.RoomId }");

                    return new RoomCurrentStateResponse()
                    {
                        Players = players,
                        Level = (PuzzleDifficulty)room.Properties.Level,
                        MaxPlayers = room.Properties.MaxPlayers,
                        NumberOfRounds = room.Properties.NumberOfRounds,
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"Failed to find info about adding room: {request.RoomId}; Reason: {ex.Message}");

                return new RoomCurrentStateResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

    }
}
