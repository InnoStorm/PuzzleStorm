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
using ServerLobby;
using Server;
using EasyNetQ;

namespace ServerLobby.Workers
{
    public class LobbyWorker : Worker
    {
        public IBus Communicator { get; set; }

        public RoomCurrentStateResponse GiveInfoAboutRoom(RoomCurrentStateRequest request)
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
        
        public CreateRoomResponse CreateNewRoom(CreateRoomRequest request)
        {
            try
            {
                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var props = new RoomProperties()
                    {
                        Level = (Difficulty)request.Level,
                        MaxPlayers = request.MaxPlayers,
                        NumberOfRounds = request.NumberOfRounds,                     
                    };

                    data.RoomProperties.Add(props);
                    data.Complete();

                    var thisPlayer = data.Players.Get(request.RequesterId);
                    List<Player> players = new List<Player>();
                    players.Add(thisPlayer);

                    var room = new Room()
                    {
                        Properties = props,
                        CurrentGame = null,
                        IsDeleted = false,
                        IsPublic = request.Password != null ? true : false,
                        Password = request.Password != null ? request.Password : null,
                        IsStarted = false,
                        ListOfPlayers = players
                    };

                    data.Rooms.Add(room);
                    data.Complete();

                    var roomStateUpdate = GenerateRoomStateUpdate(room.Id, RoomUpdateType.Created);

                    NotifyAll(roomStateUpdate);

                    return new CreateRoomResponse()
                    {
                        RoomId = room.Id,
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully added new room with Id {room.Id}."
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"Failed to add new room, Reason: {ex.Message}");

                return new CreateRoomResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public DeleteRoomResponse DeleteRoom(DeleteRoomRequest request)
        {
            try
            {
                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var room = data.Rooms.Get(request.RoomId);

                    room.IsDeleted = true;
                    data.Complete();

                    var roomStateUpdate = GenerateRoomStateUpdate(room.Id, RoomUpdateType.Deleted);

                    NotifyAll(roomStateUpdate);

                    return new DeleteRoomResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully deleted room with Id {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"Failed to delete room, Reason: {ex.Message}");

                return new DeleteRoomResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public JoinRoomResponse JoinRoom(JoinRoomRequest request)
        {
            try
            {
                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var room = data.Rooms.GetRoomWithPlayersAndProperties(request.RoomId);

                    if (!room.IsPublic && request.Password != room.Password)
                        return new JoinRoomResponse()
                        {
                            Status = OperationStatus.Failed,
                            Details = "Wrong password."
                        };

                    if (room.Properties.MaxPlayers == room.ListOfPlayers.Count)
                        return new JoinRoomResponse()
                        {
                            Status = OperationStatus.Failed,
                            Details = "This room is already full."
                        };

                    var players = room.ListOfPlayers;
                    var thisPlayer = data.Players.Get(request.RequesterId);
                    players.Add(thisPlayer);
                    data.Complete();

                    return new JoinRoomResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully joined to room with Id {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"Failed to join room, Reason: {ex.Message}");

                return new JoinRoomResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public ChangeRoomPropertiesResponse ChangeRoomProperties(ChangeRoomPropertiesRequest request)
        {
            try
            {
                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var props = data.RoomProperties.GetPropertiesOfRoom(request.RoomId);

                    props.Level = (Difficulty)request.Level;
                    props.MaxPlayers = request.MaxPlayers;
                    props.NumberOfRounds = request.MaxPlayers;
                    
                    data.Complete();
                    
                    var roomStateUpdate = GenerateRoomStateUpdate(request.RoomId, RoomUpdateType.Modified);

                    NotifyAll(roomStateUpdate);                   

                    return new ChangeRoomPropertiesResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully changed properties for room with Id {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"Failed to change properties for room, Reason: {ex.Message}");

                return new ChangeRoomPropertiesResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        #region Utils

        private void NotifyAll(RoomsStateUpdate message)
        {
            string routingKey = $"{message.UpdateType.ToString()}.{message.RoomId}";
            Communicator.Publish<RoomsStateUpdate>(message, routingKey);
        }

        private RoomsStateUpdate GenerateRoomStateUpdate(int Id, RoomUpdateType updateType)
        {
            try
            {
                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var room = data.Rooms.Find(r => r.Id == Id).Single();

                    WorkerLog($"Successfully found info about room: {Id}.");

                    DTOLibrary.SubDTOs.Player player = new DTOLibrary.SubDTOs.Player();

                    Player pl = room.ListOfPlayers.First();

                    player.IsReady = pl.IsReady;
                    player.PlayerId = pl.Id;
                    player.Username = pl.UserForPlayer.Username;

                    return new RoomsStateUpdate()
                    {
                        Creator = player,
                        IsPublic = room.IsPublic,
                        Level = (PuzzleDifficulty)room.Properties.Level,
                        MaxPlayers = room.Properties.MaxPlayers,
                        NumberOfRounds = room.Properties.MaxPlayers,
                        RoomId = room.Id,
                        Status = OperationStatus.Successfull,
                        Details = "Successful getting info about room " + room.Id,
                        UpdateType = updateType
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"Failed to update info about room: {Id}; Reason: {ex.Message}");

                return new RoomsStateUpdate()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }
        
        #endregion

    }
}
