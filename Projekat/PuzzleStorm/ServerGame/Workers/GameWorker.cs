using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Core.Domain;
using DataLayer.Persistence;
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
using System.IO;

namespace ServerGame.Workers
{
    class PlayingRoom
    {
        public Room Room;
        public int CurrentPlayerId;
        public List<int> ScoreBoard;
        public int NumberOfWellPlacedPieces;
    }

    class GameWorker : Worker
    {
        List<PlayingRoom> playingRooms = new List<PlayingRoom>();

        public GameWorker(IBus communicator) : base(communicator)
        {
            InitializeGameWorker();
        }
        
        public void InitializeGameWorker()
        {
            try
            {
                using (var data = WorkersUnitOfWork)
                {
                    List<Room> rooms = data.Rooms.GetAllPlaying().ToList();
                    foreach (Room r in rooms)
                    {
                        var playingRoom = new PlayingRoom();
                        playingRoom.Room = r;
                        playingRoom.CurrentPlayerId = r.ListOfPlayers.First().Id;
                        playingRoom.ScoreBoard = new List<int>();
                        for (int i = 0; i < r.ListOfPlayers.Count; ++i)
                            playingRoom.ScoreBoard.Add(0);

                        playingRooms.Add(playingRoom);
                    }

                    Log($"[SUCCESS] Initializing game worker.");
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Initializing game worker. Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);
            }
        }
        
        public int NextPlayer(int roomId, int currentPlayerId)
        {
            var playingRoom = playingRooms.Find(x => x.Room.Id == roomId);
            Player currentPlayer = playingRoom.Room.ListOfPlayers.Where(x => x.Id == currentPlayerId).First();
            int indexOfNext = (playingRoom.Room.ListOfPlayers.IndexOf(currentPlayer) + 1) % playingRoom.Room.ListOfPlayers.Count;
            playingRoom.CurrentPlayerId = playingRoom.Room.ListOfPlayers.ElementAt(indexOfNext).Id;
            return playingRoom.CurrentPlayerId;
        }
       
        public void AddPointsToPlayer(Player p, int roomId)
        {
            var playingRoom = playingRooms.Find(x => x.Room.Id == roomId);
            int index = playingRoom.Room.ListOfPlayers.IndexOf(p);

            var points = playingRoom.ScoreBoard.ElementAt(index);
            points += 5;
        }


        #region Worker Functions

        public LoadGameResponse LoadGame(LoadGameRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var room = data.Rooms.Get(request.RoomId);
                    var game = room.CurrentGame;
                    if (game == null)
                        throw new Exception($"Game for room {request.RoomId} not found!");
   
                    List<String> partsPaths = game.PuzzleForGame.ListOfPieces.Select(x => x.PartPath).ToList();

                    //List<String> partsPaths = new List<String>();
                    //foreach (PieceData piece in puzzle.ListOfPieces)
                    //    partsPaths.Add(piece.PartPath);
                    
                    List<DTOLibrary.SubDTOs.Player> list = new List<DTOLibrary.SubDTOs.Player>();
                    foreach (Player p in game.RoomForThisGame.ListOfPlayers)
                    {
                        list.Add(new DTOLibrary.SubDTOs.Player()
                        {
                            IsReady = p.IsReady,
                            PlayerId = p.Id,
                            Username = p.Username
                        });
                    }
                                       
                    return new LoadGameResponse()
                    {
                        GameId = game.Id,
                        PiecesPaths = partsPaths,
                        CurrentPlayerId = game.RoomForThisGame.Owner.Id,
                        ListOfPlayers = list,
                        Status = OperationStatus.Successfull,
                        Details = $"Room {request.RoomId} successfully started."
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Starting room {request.RoomId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new LoadGameResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public StartGameResponse StartGame(StartGameRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var room = data.Rooms.Get(request.RoomId);
                    if (room == null)
                        throw new Exception($"Room with ID {request.RoomId} not found!");

                    var puzzle = data.Puzzles.GetPuzzle(ConvertDifficultyToInt(room.Difficulty)); //todo randomize //za sad vraca prvu odgovarajucu puzzlu koju nadje u bazi
                    var game = new Game
                    {
                        PuzzleForGame = puzzle,
                        RoomForThisGame = room
                    };
                    data.Games.Add(game);
                    data.Complete();

                    Log($"[SUCCESS] Player {request.RequesterId} successfully started room {request.RoomId}");

                    return new StartGameResponse()
                    {
                        Details = $"Room {room.Id} successfully started.",
                        Status = OperationStatus.Successfull,
                        GameId = game.Id,
                        CommunicationKey = "NOT IMPLEMENTED YET"
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Starting room {request.RoomId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new StartGameResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }

        }

        //public MakeAMoveResponse MakeAMove(MakeAMoveRequest request)
        //{
        //    try
        //    {
        //        using (var data = WorkersUnitOfWork)
        //        {
        //            var player = data.Players.Get(request.RequesterId);

        //            MakeAMoveResponse response = new MakeAMoveResponse
        //            {
        //                Status = OperationStatus.Successfull,
        //                Details = "Successfull"
        //            };

        //            if (request.SelectedPartNumber != request.TablePlaceNumber)
        //                response.CurrentPlayerId = NextPlayer(request.RoomId, request.RequesterId);
        //            else
        //            {
        //                response.CurrentPlayerId = request.RequesterId;
        //                //player.Score += 5;
        //                //AddPointsToPlayer(player, request.RoomId);
        //            }
        //            //response.ScoreBoard = 

        //            Log($"[SUCCESS] Making move. Requester: {request.RequesterId}");
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log($"[FAILED] Making move. Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

        //        return new MakeAMoveResponse()
        //        {
        //            Status = OperationStatus.Failed,
        //            Details = ex.Message
        //        };
        //    }
        //}

        #endregion

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
    }
}
