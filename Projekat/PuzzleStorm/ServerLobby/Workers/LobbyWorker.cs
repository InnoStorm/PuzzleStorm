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
using DTOLibrary.SubDTOs;
using Server.Workers;
using Server;
using EasyNetQ;
using StormCommonData.Enums;
using Player = DataLayer.Core.Domain.Player;

namespace ServerLobby.Workers
{
    public class LobbyWorker : Worker
    {
        public IBus Communicator { get; set; }
        
        public CreateRoomResponse CreateNewRoom(CreateRoomRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var creator = data.Players.Get(request.RequesterId);
                    if (creator == null)
                        throw new Exception($"Can't find player/creator with ID {request.RequesterId}");

                    var newRoom = new Room
                    {
                        Difficulty = request.Difficulty,
                        IsPublic = request.Password == string.Empty,
                        Password = request.Password,
                        CurrentGame = null,
                        State = RoomState.Available,
                        NumberOfRounds = request.NumberOfRounds,
                        Owner = creator,
                        MaxPlayers = request.MaxPlayers,
                        
                    };

                    newRoom.ListOfPlayers.Add(creator);
                    data.Rooms.Add(newRoom);
                    data.Complete();

                    var updateMessage = GenerateRoomStateUpdate(newRoom, RoomUpdateType.Created);
                    NotifyAll(updateMessage);

                    WorkerLog($"[SUCCESS] Created new room with ID: {newRoom.Id}.");

                    return new CreateRoomResponse()
                    {
                        RoomId = newRoom.Id,
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully added new room with Id {newRoom.Id}."
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"[FAILED] Creating room. Reason: {ExceptionStack(ex)}", LogMessageType.Error);

                return new CreateRoomResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public GetAllRoomsResponse GetAllRooms(GetAllRoomsRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (var data = CreateUnitOfWork())
                {
                    var availableRooms = data.Rooms.GetAllAvailable().ToList();

                    GetAllRoomsResponse response = new GetAllRoomsResponse
                    {
                        Status = OperationStatus.Successfull,
                        Details = "Successfull",
                        List = new List<RoomInfo>(availableRooms.Count)
                    };

                    foreach (Room room in availableRooms)
                    {
                        response.List.Add(new RoomInfo()
                        {
                            RoomId = room.Id,
                            CreatorUsername = room.Owner.Username,
                            Difficulty = room.Difficulty,
                            MaxPlayers = room.MaxPlayers,
                            NumberOfRounds = room.NumberOfRounds
                        });
                    }

                    WorkerLog($"[SUCCESS] Request for list of all available rooms. Requester: {request.RequesterId}");

                    return response;
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"[FAILED] Request for list all available rooms. Reason: {ExceptionStack(ex)}", LogMessageType.Error);

                return new GetAllRoomsResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message,
                    List = null
                };
            }
        }

        public CancelRoomResponse CancelRoom(CancelRoomRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var room = data.Rooms.Get(request.RoomId);

                    if (room == null)
                        throw new Exception($"Room with id {request.RoomId} not found!");

                    room.ListOfPlayers = null;
                    data.Rooms.Remove(room);
                    data.Complete();
                    
                    var roomStateUpdate = GenerateRoomStateUpdate(room, RoomUpdateType.Deleted);

                    
                    NotifyAll(roomStateUpdate);
                    WorkerLog($"[SUCCESS] Cancel room ID: {request.RoomId} by {request.RequesterId}");

                    return new CancelRoomResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully deleted room with Id {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"[FAILED] Deleting room {request.RoomId}, Reason: {ExceptionStack(ex)}", LogMessageType.Error);

                return new CancelRoomResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public RoomCurrentStateResponse GiveInfoAboutRoom(RoomCurrentStateRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var room = data.Rooms.Get(request.RoomId);
                    if (room == null)
                        throw new Exception($"Room with id {request.RoomId} not found!");

                    var response = new RoomCurrentStateResponse()
                    {
                        Difficulty = room.Difficulty,
                        MaxPlayers = room.MaxPlayers,
                        NumberOfRounds = room.MaxPlayers,
                        Details = "Successful",
                        Status = OperationStatus.Successfull
                    };

                    foreach (Player player in room.ListOfPlayers)
                    {
                        var playerDTO = new DTOLibrary.SubDTOs.Player()
                        {
                            PlayerId = player.Id,
                            Username = player.Username,
                            IsReady = player.IsReady,
                        };

                        response.Players.Add(playerDTO);
                    }
                    
                    WorkerLog($"[SUCCESS] Current state for room: { request.RoomId }");

                    return response;
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"[FAILED] Current state for room: { request.RoomId }; Reason: {ExceptionStack(ex)}", LogMessageType.Error);

                return new RoomCurrentStateResponse()
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
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var room = data.Rooms.Get(request.RoomId);
                    if (room == null)
                        throw new Exception($"Room with ID {request.RoomId} not found!");

                    var joiner = data.Players.Get(request.RequesterId);
                    if (joiner == null)
                        throw new Exception($"Cannot find user with {request.RequesterId}!");

                    if (room.State != RoomState.Available)
                        throw new Exception($"Room is not joinable.");

                    if (room.ListOfPlayers.Count >= room.MaxPlayers)
                        throw new Exception($"Room is full. Cannot join.");

                    if (!room.IsPublic && room.Password != request.Password)
                        throw new Exception($"Wrong password!");

                    if (room.Owner == joiner || room.ListOfPlayers.Contains(joiner))
                        throw new Exception("You are already in room!");


                    room.ListOfPlayers.Add(joiner);
                    joiner.IsReady = false;
                    joiner.Score = 0;
                    data.Complete();
                    
                    WorkerLog($"[SUCCESS] Player {request.RoomId} joined in room {request.RoomId}");
                    return new JoinRoomResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully joined to room with Id {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"[FAILED] Player {request.RoomId} failed to join in {request.RoomId}, Reason: {ExceptionStack(ex)}", LogMessageType.Error);

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
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = CreateUnitOfWork())
                {
                    var room = data.Rooms.Get(request.RoomId);
                    if (room == null)
                        throw new Exception($"Room with ID {request.RoomId} not found!");

                    if (room.ListOfPlayers.Count > request.MaxPlayers)
                        throw new Exception($"Current number of joined players is greater than new MaxPlayers value!");


                    room.Difficulty = request.Difficulty;
                    room.MaxPlayers = request.MaxPlayers;
                    room.NumberOfRounds = request.NumberOfRounds;

                    data.Complete();

                    var updateMessage = GenerateRoomStateUpdate(room, RoomUpdateType.Modified);

                    WorkerLog($"[SUCCESS] Room properties changed for room {request.RoomId}");
                    NotifyAll(updateMessage);                   

                    return new ChangeRoomPropertiesResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully changed properties for room with Id {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"[FAILED] Chaning properties for room {request.RoomId}, Reason: {ExceptionStack(ex)}", LogMessageType.Error);

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

        private RoomsStateUpdate GenerateRoomStateUpdate(Room room, RoomUpdateType updateType)
        {
            if (updateType == RoomUpdateType.Deleted)
                return new RoomsStateUpdate()
                {
                    Status = OperationStatus.Successfull,
                    RoomId = room.Id,
                    UpdateType = RoomUpdateType.Deleted,
                    Details = "Room is removed."
                };

            return new RoomsStateUpdate()
            {
                Creator = new DTOLibrary.SubDTOs.Player()
                {
                    Username = room.Owner.Username,
                    IsReady = room.Owner.IsReady,
                    PlayerId = room.Owner.Id
                },
                IsPublic = room.IsPublic,
                Level = room.Difficulty,
                MaxPlayers = room.MaxPlayers,
                NumberOfRounds = room.MaxPlayers,
                RoomId = room.Id,
                Status = OperationStatus.Successfull,
                Details = "Successful getting info about room " + room.Id,
                UpdateType = updateType
            };
        }

        #endregion
    }
}
