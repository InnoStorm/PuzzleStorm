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
    class GameWorker : Worker
    {
        #region Static

        public static LoadGameResponse LoadGame(LoadGameRequest request)
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
                //Log($"[FAILED] Starting room {request.RoomId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new LoadGameResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        #endregion


        private Room HandledRoom { get; set; }
        private Player CurrentPlayer { get; set; }
        private List<Tuple<Player, int>> Scoreboard { get; set; }
        private int NumberOfSolvedPieces { get; set; }
        private int CurrentRound { get; set; }
        private string MovesQueueName { get; set; }

        public GameWorker(IBus communicator) : base(communicator)
        {

        }

        #region Worker Functions

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

                    HandledRoom = room;

                    var puzzle = data.Puzzles.GetPuzzle(ConvertDifficultyToInt(room.Difficulty));
                    var game = new Game
                    {
                        PuzzleForGame = puzzle,
                        RoomForThisGame = room
                    };
                    data.Games.Add(game);
                    data.Complete();


                    CurrentPlayer = HandledRoom.Owner;
                    Scoreboard = new List<Tuple<Player, int>>(HandledRoom.MaxPlayers);
                    foreach (Player player in HandledRoom.ListOfPlayers)
                    {
                        Scoreboard.Add(new Tuple<Player, int>(player, 0));
                    }

                    NumberOfSolvedPieces = 0;
                    CurrentRound = 1;
                    MovesQueueName = RouteGenerator.GameUpdates.GamePlay.GenerateMovesQueueName();

                    //TODO comunicator receive init


                    Log($"[SUCCESS] Player {request.RequesterId} successfully started room {request.RoomId}");

                    return new StartGameResponse()
                    {
                        Details = $"Room {room.Id} successfully started.",
                        Status = OperationStatus.Successfull,
                        GameId = game.Id,
                        CommunicationKey = MovesQueueName
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Starting room {request.RoomId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);
                //ReleaseWorker()
                return new StartGameResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }

        }

        public ContinueGameResponse ContinueGame(ContinueGameRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    var puzzle = data.Puzzles.Get(HandledRoom.CurrentGame.Id + 3); //vraca sledcu puzzluuuuu :D

                    data.Games.Remove(HandledRoom.CurrentGame);
                    data.Complete();

                    var game = new Game
                    {
                        PuzzleForGame = puzzle,
                        RoomForThisGame = HandledRoom
                    };

                    data.Games.Add(game);
                    data.Complete();

                    Log($"[SUCCESS] Player {request.RequesterId} successfully continued room {request.RoomId}");

                    return new ContinueGameResponse()
                    {
                        Details = $"Room {HandledRoom.Id} successfully continued.",
                        Status = OperationStatus.Successfull,
                        GameId = game.Id,
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Starting room {request.RoomId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new ContinueGameResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }

        }


        //public void AddPointsToPlayer(Player p, int roomId)
        //{
        //    var playingRoom = playingRooms.Find(x => x.Room.Id == roomId);
        //    int index = playingRoom.Room.ListOfPlayers.IndexOf(p);

        //    var points = playingRoom.ScoreBoard.ElementAt(index);
        //    points += 5;
        //}
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

        #region NotifyClients

        private void NotifyAll(GameUpdate message, int RoomId)
        {
            string routingKey = RouteGenerator.GameUpdates.GamePlay.Set.FromEnum(message.UpdateType, RoomId);
            Communicator.Publish<GameUpdate>(message, routingKey);

            Log($"NOTIFY_ALL: {routingKey}");
        }

        #endregion

        #region Utils

        private GameUpdate GenerateGameUpdate(Player currentPlayer, Move playedMove, GamePlayUpdateType updateType)
        {
            return new GameUpdate
            {
                Status = OperationStatus.Successfull,
                Details = "Room is updated.",
                UpdateType = updateType,
                CurrentPlayer = ConvertPlayerToSubDTOPlayer(currentPlayer),
                PlayedMove = playedMove,
                Scoreboard = ConvertScoreboardToSmallScoreboard(Scoreboard)
            };
        }

        #endregion

        #region Helper Functions

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

        private DTOLibrary.SubDTOs.Player ConvertPlayerToSubDTOPlayer(Player player)
        {
            return new DTOLibrary.SubDTOs.Player()
            {
                Username = player.Username,
                IsReady = player.IsReady,
                PlayerId = player.Id
            };
        }

        private Scoreboard ConvertScoreboardToSmallScoreboard(List<Tuple<Player, int>> sb)
        {
            Scoreboard sc = new DTOLibrary.SubDTOs.Scoreboard();
            foreach (Tuple<Player, int> t in sb)
            {
                sc.Scores.Add(new Tuple<DTOLibrary.SubDTOs.Player, int>(ConvertPlayerToSubDTOPlayer(t.Item1), t.Item2));
            }
            return sc;
        }

        private bool EndOfTheRound()
        {
            return NumberOfSolvedPieces == HandledRoom.CurrentGame.PuzzleForGame.NumberOfPieces;
        }

        private bool EndOfTheGame()
        {
            return CurrentRound == HandledRoom.NumberOfRounds + 1;
        }

        public void HandleMove(MakeAMoveRequest move)
        {
            Move playedMove = move.MoveToPlay;
            playedMove.IsSuccessfull = (move.MoveToPlay.PositionFrom == move.MoveToPlay.PositionTo);
            if (playedMove.IsSuccessfull)
                ++NumberOfSolvedPieces;

            GameUpdate gameUpdate;

            if (EndOfTheRound())
            {
                ++CurrentRound;
                if (EndOfTheGame())
                    gameUpdate = GenerateGameUpdate(null, playedMove, GamePlayUpdateType.GameOver);
                gameUpdate = GenerateGameUpdate(null, playedMove, GamePlayUpdateType.RoundOver);
            }

            Player currentPlayer = playedMove.IsSuccessfull ? CurrentPlayer : NextPlayer(move.RoomId, CurrentPlayer);
            gameUpdate = GenerateGameUpdate(currentPlayer, playedMove, GamePlayUpdateType.Playing);

            NotifyAll(gameUpdate, move.RoomId);
        }

        public Player NextPlayer(int roomId, Player currentPlayer)
        {
            //var playingRoom = playingRooms.Find(x => x.Room.Id == roomId);
            //Player currentPlayer = playingRoom.Room.ListOfPlayers.First(x => x.Id == currentPlayerId);
            //int indexOfNext = (playingRoom.Room.ListOfPlayers.IndexOf(currentPlayer) + 1) % playingRoom.Room.ListOfPlayers.Count;
            //playingRoom.CurrentPlayerId = playingRoom.Room.ListOfPlayers.ElementAt(indexOfNext).Id;
            //return playingRoom.CurrentPlayerId;

            return null;
        }

        #endregion


    }
}
