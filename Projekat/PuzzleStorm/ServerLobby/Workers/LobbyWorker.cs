using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Core.Domain;
using DataLayer.Persistence;
using StormCommonData.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.Broadcasts;
using DTOLibrary.SubDTOs;
using Server.Workers;
using Server;
using EasyNetQ;
using StormCommonData;
using Player = DataLayer.Core.Domain.Player;
using System.IO;

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
                        IsPublic = string.IsNullOrEmpty(request.Password),
                        Password = request.Password,
                        CurrentGame = null,
                        State = RoomState.Available,
                        NumberOfRounds = request.NumberOfRounds,
                        Owner = creator,
                        MaxPlayers = request.MaxPlayers,

                    };

                    newRoom.ListOfPlayers.Add(creator);
                    data.Rooms.Add(newRoom);
                    creator.IsReady = true;
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
                        Difficulty = room.Difficulty,
                        MaxPlayers = room.MaxPlayers,
                        NumberOfRounds = room.NumberOfRounds,
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


        public StartRoomResponse StartRoom(StartRoomRequest request) // Cheking are players ready. If yes, setting room to state playing and notify all, they will call Load game
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var room = data.Rooms.Get(request.RoomId);

                    if (room.Owner.Id != request.RequesterId)
                        throw new Exception("[FAILED] Only owner cant start room!");

                    bool allReady = room.ListOfPlayers.All(x => x.IsReady == true);
                    if (!allReady)
                        throw new Exception("Not all players are ready!");

                    StartGameRequest newGameRequest = new StartGameRequest()
                    {
                        RequesterId = request.RequesterId,
                        RoomId = room.Id
                    };

                    //todo handle timeout
                    var response = Communicator.Request<StartGameRequest, StartGameResponse>(newGameRequest);
                    if (response.Status != OperationStatus.Successfull)
                        throw new Exception("Failed to create new game! Reason: " + response.Details);

                    room.State = RoomState.Playing;
                    data.Complete();

                    var updateMessagePlaying = GenerateRoomStateUpdate(room, RoomUpdateType.Started);
                    NotifyAll(updateMessagePlaying);

                    return new StartRoomResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = $"Room {request.RoomId} is started.",
                        CreatedGame = response
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Not all players are ready.", LogMessageType.Error);

                return new StartRoomResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = $"{ex.Message}"
                };
            }
        }

        public ChangeStatusResponse PlayerChangeStatus(ChangeStatusRequest request)
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

                    var updateMessage = GenerateRoomPlayerUpdate(player, RoomPlayerUpdateType.ChangedStatus);

                    Log($"[SUCCESS] Status changed for player {request.RequesterId}");
                    NotifyAll(updateMessage);

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

        public JoinRoomResponse PlayerJoinRoom(JoinRoomRequest request)
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

                    var updateMessage = GenerateRoomPlayerUpdate(joiner, RoomPlayerUpdateType.Joined);
                    NotifyAll(updateMessage);

                    if (room.ListOfPlayers.Count == room.MaxPlayers)
                    {
                        room.State = RoomState.Full;
                        data.Complete();

                        Log($"[INFO] Room {request.RequesterId} is full.");
                        var updateMessageFilledRoom = GenerateRoomStateUpdate(room, RoomUpdateType.Filled);
                        NotifyAll(updateMessageFilledRoom);
                    }

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

        public LeaveRoomResponse PlayerLeaveRoom(LeaveRoomRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var player = data.Players.Get(request.RequesterId);
                    if (player == null)
                        throw new Exception($"Player with ID {request.RequesterId} not found!");

                    if (player.CurrentRoom.State == RoomState.Full)
                    {
                        player.CurrentRoom.State = RoomState.Available;

                        Log($"[INFO] Room {request.RoomId} is available again [Not full]");
                        var updateMessageRoomBecameAvailable = GenerateRoomStateUpdate(player.CurrentRoom, RoomUpdateType.BecameAvailable);
                        NotifyAll(updateMessageRoomBecameAvailable);
                    }

                    player.Score = 0;
                    player.IsReady = false;
                    player.CurrentRoom = null;
                    data.Complete();


                    Log($"[SUCCESS] Player {request.RequesterId} successfully left room {request.RoomId}");
                    var updateMessage = GenerateRoomPlayerUpdate(player, RoomPlayerUpdateType.LeftRoom);
                    NotifyAll(updateMessage);

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



        public GetAllRoomsResponse GetAvailableRooms(GetAllRoomsRequest request)
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
                            NumberOfRounds = room.NumberOfRounds,
                            IsPublic = room.IsPublic
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

        public RoomCurrentStateResponse GetRoomState(RoomCurrentStateRequest request)
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
                        NumberOfRounds = room.NumberOfRounds,
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


        #region NotifyClients

        private void NotifyAll(RoomsStateUpdate message)
        {
            string routingKey = RouteGenerator.RoomUpdates.Room.Set.FromEnum(message.UpdateType, message.RoomId);
            Communicator.Publish<RoomsStateUpdate>(message, routingKey);

            Log($"NOTIFY_ALL: {routingKey}");
        }

        private void NotifyAll(RoomPlayerUpdate message)
        {
            string routingKey = RouteGenerator.RoomUpdates.InRoom.Set.FromEnum(message.UpdateType, message.RoomId);
            Communicator.Publish<RoomPlayerUpdate>(message, routingKey);

            Log($"NOTIFY_ALL: {routingKey}");
        }

        #endregion


        #region Utils

        private RoomPlayerUpdate GenerateRoomPlayerUpdate(Player player, RoomPlayerUpdateType updateType)
        {
            return new RoomPlayerUpdate
            {
                Status = OperationStatus.Successfull,
                Details = "Room is updated.",
                UpdateType = updateType,
                Player = new DTOLibrary.SubDTOs.Player()
                {
                    Username = player.Username,
                    IsReady = player.IsReady,
                    PlayerId = player.Id
                },
                RoomId = player.CurrentRoom.Id
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
                NumberOfRounds = room.NumberOfRounds,
                RoomId = room.Id,
                Status = OperationStatus.Successfull,
                Details = "Successful getting info about room " + room.Id,
                UpdateType = updateType
            };
        }

        public AddPuzzlesResponse AddPuzzlesToDatabase(AddPuzzlesRequest request)
        {
            try
            {
                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    for (int i = 1; i <= 5; ++i)
                    {
                        for (int j = 4; j <= 6; ++j)
                        {
                            string folderForPuzzle = @"..\..\..\Images\" + i;
                            string nameOfPicture = Directory.GetFiles(folderForPuzzle)[0].Contains("ini") ? Directory.GetFiles(folderForPuzzle)[1] : Directory.GetFiles(folderForPuzzle)[0];

                            PuzzleData puzzle = new PuzzleData()
                            {
                                PicturePath = nameOfPicture,
                                NumberOfPieces = j * j
                            };

                            data.Puzzles.Add(puzzle);
                            data.Complete();

                            Log($"[SUCCESS] Successfully created puzzle with Id {puzzle.Id}");
                            
                            string folderForParts = folderForPuzzle + "/" + j * j;
                            //string folderForParts = "../Images/"+ i + "/" + j * j;
                            string[] namesOfParts = Directory.GetFiles(folderForParts);
                            var listOfNames = new List<string>(namesOfParts);

                            if (listOfNames.ElementAt(0).Contains("ini"))
                                listOfNames.RemoveAt(0);

                            for (int k = 0; k < listOfNames.Count; ++k)
                            {
                                PieceData piece = new PieceData()
                                {
                                    PartPath = listOfNames[k].Remove(0, 6).Replace("\\", "/"),
                                    SeqNumber = k + 1,
                                    ParentPuzzle = puzzle
                                };

                                data.Pieces.Add(piece);
                                data.Complete();
                            }

                            Log($"[SUCCESS] Successfully added pieces for puzzle with Id {puzzle.Id}");
                        }
                    }

                    return new AddPuzzlesResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = "Successfull."
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Problem: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new AddPuzzlesResponse()
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

        #endregion
    }
}
