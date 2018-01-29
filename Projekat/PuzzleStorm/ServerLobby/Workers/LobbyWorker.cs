﻿using System;
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
using StormCommonData;
using StormCommonData.Enums;
using Player = DataLayer.Core.Domain.Player;

namespace ServerLobby.Workers
{
    class LobbyWorker : Worker
    {
        public LobbyWorker(IBus communicator) : base(communicator)
        {
        }

        public CreateRoomResponse CreateNewRoom(CreateRoomRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
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

                    Log($"[SUCCESS] Created new room with ID: {newRoom.Id}.");

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
                Log($"[FAILED] Creating room. Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

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

                using (var data = WorkersUnitOfWork)
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

                    Log($"[SUCCESS] Request for list of all available rooms. Requester: {request.RequesterId}");

                    return response;
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Request for list all available rooms. Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

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

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var room = data.Rooms.Get(request.RoomId);

                    if (room == null)
                        throw new Exception($"Room with id {request.RoomId} not found!");

                    room.ListOfPlayers.Clear();
                    data.Complete();
                    data.Rooms.Remove(room);
                    data.Complete();
                    
                    var roomStateUpdate = GenerateRoomStateUpdate(room, RoomUpdateType.Deleted);

                    
                    NotifyAll(roomStateUpdate);
                    Log($"[SUCCESS] Cancel room ID: {request.RoomId} by {request.RequesterId}");

                    return new CancelRoomResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully deleted room with Id {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Deleting room {request.RoomId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

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

                using (UnitOfWork data = WorkersUnitOfWork)
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
                    
                    Log($"[SUCCESS] Current state for room: { request.RoomId }");

                    return response;
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Current state for room: { request.RoomId }; Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

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

                using (UnitOfWork data = WorkersUnitOfWork)
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

                    if (room.ListOfPlayers.Count == room.MaxPlayers)
                    {
                        room.State = RoomState.Full;
                        data.Complete();

                        var updateMessageFilledRoom = GenerateRoomStateUpdate(room, RoomUpdateType.Filled);
                        NotifyAll(updateMessageFilledRoom);
                    }
                        
                    var updateMessage = GenerateRoomPlayerUpdate(room.Id, joiner.Id, RoomPlayerUpdateType.Joined);
                    NotifyChangesInRoom(updateMessage);
                    
                    Log($"[SUCCESS] Player {request.RoomId} joined in room {request.RoomId}");
                    return new JoinRoomResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully joined to room with Id {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Player {request.RoomId} failed to join in {request.RoomId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

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

                using (UnitOfWork data = WorkersUnitOfWork)
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

                    Log($"[SUCCESS] Room properties changed for room {request.RoomId}");
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
                Log($"[FAILED] Chaning properties for room {request.RoomId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new ChangeRoomPropertiesResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public ChangeStatusResponse ChangeStatus(ChangeStatusRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var player = data.Players.Get(request.RequesterId);
                    if (player == null)
                        throw new Exception($"Player with ID {request.RequesterId} not found!");

                    player.IsReady = request.IAmReady;
                    data.Complete();

                    var updateMessage = GenerateRoomPlayerUpdate(player.CurrentRoom.Id, player.Id, RoomPlayerUpdateType.ChangedStatus);

                    Log($"[SUCCESS] Status changed for player {request.RequesterId}");
                    NotifyChangesInRoom(updateMessage);

                    return new ChangeStatusResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Successfully changed status for player with Id {request.RequesterId}."
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Changing status for player {request.RequesterId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new ChangeStatusResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public LeaveRoomResponse LeaveRoom(LeaveRoomRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var player = data.Players.Get(request.RequesterId);
                    if (player == null)
                        throw new Exception($"Player with ID {request.RequesterId} not found!");

                    player.Score = 0;
                    player.IsReady = false;
                    player.CurrentRoom = null;
                    data.Complete();

                    if (player.CurrentRoom.State == RoomState.Full)
                    {
                        player.CurrentRoom.State = RoomState.Available;
                        data.Complete();

                        var updateMessageRoomBecameAvailable = GenerateRoomStateUpdate(player.CurrentRoom, RoomUpdateType.BecameAvailable);
                        NotifyAll(updateMessageRoomBecameAvailable);
                    }

                    var updateMessage = GenerateRoomPlayerUpdate(request.RoomId, player.Id, RoomPlayerUpdateType.LeftRoom);
                    NotifyChangesInRoom(updateMessage);

                    Log($"[SUCCESS] Player {request.RequesterId} successfully left room {request.RoomId}");
                    NotifyChangesInRoom(updateMessage);

                    return new LeaveRoomResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Player {request.RequesterId} successfully left room {request.RoomId}."
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Changing status for player {request.RequesterId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new LeaveRoomResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        private int ConvertDifficultyToInt(PuzzleDifficulty diff)
        {
            switch (diff)
            {
                case PuzzleDifficulty.Easy:
                    return 16;
                case PuzzleDifficulty.Medium:
                    return 25;
                case PuzzleDifficulty.Hard:
                    return 36;
                default:
                    return 0;
            }

        }

        public StartRoomResponse StartRoom(StartRoomRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var room = data.Rooms.Get(request.RoomId);
                    if (room == null)
                        throw new Exception($"Room with ID {request.RoomId} not found!");

                    var puzzle = data.Puzzles.GetPuzzle(ConvertDifficultyToInt(room.Difficulty)); //za sad vraca prvu odgovarajucu puzzlu koju nadje u bazi
                    var game = new Game
                    {
                        PuzzleForGame = puzzle,
                    };
                    data.Games.Add(game);
                    data.Complete();

                    room.CurrentGame = game;
                    room.State = RoomState.Playing;
                    data.Complete();

                    var updateMessage = GenerateRoomStateUpdate(room, RoomUpdateType.Started);
                    NotifyAll(updateMessage);

                    Log($"[SUCCESS] Player {request.RequesterId} successfully started room {request.RoomId}");

                    return new StartRoomResponse()
                    {
                        GameId = game.Id,
                        PuzzleId = puzzle.Id,
                        Status = OperationStatus.Successfull,
                        Details = $"Player {request.RequesterId} successfully left room {request.RoomId}"
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Changing status for player {request.RequesterId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new StartRoomResponse()
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

        private void NotifyChangesInRoom(RoomPlayerUpdate message)
        {
            // npr. InRoom.4.Joined.20
            string routingKey = $"InRoom.{message.RoomId}.{message.UpdateType.ToString()}.{message.PlayerId}";
            Communicator.Publish<RoomPlayerUpdate>(message, routingKey);
        }

        private RoomPlayerUpdate GenerateRoomPlayerUpdate(int roomId, int playerId, RoomPlayerUpdateType updateType)
        {
            return new RoomPlayerUpdate
            {
                Status = OperationStatus.Successfull,
                Details = "Room is updated.",
                UpdateType = updateType,
                PlayerId = playerId,
                RoomId = roomId
            };
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
