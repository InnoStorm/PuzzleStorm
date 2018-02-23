using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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
using EasyNetQ.Consumer;
using StormCommonData;
using StormCommonData.Enums;
using Player = DataLayer.Core.Domain.Player;

namespace ServerGame.Workers
{
    class GameWorker : Worker
    {
        private ServerGame Server { get; set; }

        private Room HandledRoom { get; set; }
        private Player CurrentPlayer { get; set; }
        private List<Tuple<Player, int>> Scoreboard { get; set; }
        private int NumberOfSolvedPieces { get; set; }
        private int CurrentRound { get; set; }
        private string ReceiveQueueName { get; set; }
        private List<Player> LoadedPlayers { get; set; }
        private int NumberOfPieces { get; set; }

        public GameWorker(IBus communicator, ServerGame server) : base(communicator)
        {
            Server = server;
            
        }
        
        #region Communication

        private void PrepareComunication()
        {
            ReceiveQueueName = ReceiveQueueName = RouteGenerator.GameUpdates.GamePlay.GenerateReceiveQueueName();

            Communicator.Receive(ReceiveQueueName, x =>
            {
                x.Add<MakeAMoveRequest>(OnReceivedMessage);
                x.Add<LoadGameRequest>(OnReceivedMessage);
            });

            
        }

        private void RespondDirectly(int playerId, LoadGameResponse response)
        {
            Communicator.Send<LoadGameResponse>(
                RouteGenerator.GameUpdates.GamePlay.DirectMessageQueue(playerId),
                response
            );
        }

        private void NotifyAll(GameUpdate message)
        {
            string routingKey = RouteGenerator.GameUpdates.GamePlay.Set.FromEnum(message.UpdateType, HandledRoom.Id);

            Communicator.Publish<GameUpdate>(message, routingKey);

            Log($"NOTIFY_ALL: {routingKey}");
        }

        private void NotifyAllAboutLoadedUser()
        {
            Communicator.Publish(new LoadingGameUpdate()
            {
                Status = OperationStatus.Successfull,
                Details = "Update for currently loaded players",
                LoadedPlayers = LoadedPlayers.Select(ConvertPlayerToSubDTOPlayer).ToList(),
                AreAllLoaded = LoadedPlayers.Count == HandledRoom.ListOfPlayers.Count
            });
        }

        private Task OnReceivedMessage(MakeAMoveRequest moveRequest)
        {
            return Task.Factory.StartNew(() =>
            {
                HandleMove(moveRequest);
            });
        }

        private Task OnReceivedMessage(LoadGameRequest loadRequest)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var response = LoadGame(loadRequest);
                    RespondDirectly(loadRequest.RequesterId, response);
                }
                catch (Exception ex)
                {
                    Log($"Failed to respond directly to {loadRequest.RequesterId} for LoadGameRequest!\n{ex.Message}");
                }
            });
        }

        #endregion

        #region Worker Functions

        public StartGameResponse StartGame(StartGameRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    HandledRoom = data.Rooms.Get(request.RoomId) ?? 
                        throw new Exception($"Room with ID {request.RoomId} not found!");

                    var puzzle = data.Puzzles.GetPuzzle(ConvertDifficultyToInt(HandledRoom.Difficulty));
                    var game = new Game
                    {
                        PuzzleForGame = puzzle,
                        RoomForThisGame = HandledRoom
                    };
                    data.Games.Add(game);
                    data.Complete();

                    CurrentPlayer = HandledRoom.Owner;

                    Scoreboard = new List<Tuple<Player, int>>(HandledRoom.ListOfPlayers.Count);
                    foreach (Player player in HandledRoom.ListOfPlayers)
                    {
                        Scoreboard.Add(new Tuple<Player, int>(player, 0));
                    }

                    NumberOfSolvedPieces = 0;
                    CurrentRound = 1;
                    LoadedPlayers = new List<Player>(HandledRoom.ListOfPlayers.Count);
                    NumberOfPieces = HandledRoom.CurrentGame.PuzzleForGame.NumberOfPieces;

                    PrepareComunication();
                    
                    Log($"[SUCCESS] Player {request.RequesterId} successfully started room {request.RoomId}");
                    return new StartGameResponse()
                    {
                        Details = $"Room {HandledRoom.Id} successfully started.",
                        Status = OperationStatus.Successfull,
                        GameId = game.Id,
                        CommunicationKey = ReceiveQueueName
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

                    List<string> partsPaths = game.PuzzleForGame.ListOfPieces.Select(x => x.PartPath).ToList();

                    List<DTOLibrary.SubDTOs.Player> list = game.RoomForThisGame.ListOfPlayers.Select(ConvertPlayerToSubDTOPlayer).ToList();

                    lock (LoadedPlayers)
                    {
                        LoadedPlayers.Add(room.ListOfPlayers.Single(x => x.Id == request.RequesterId));
                        NotifyAllAboutLoadedUser();
                    }
                    
                    Log($"Player {request.RequesterId} successfully loaded");

                    return new LoadGameResponse()
                    {
                        GameId = game.Id,
                        PiecesPaths = partsPaths,
                        CurrentPlayerId = game.RoomForThisGame.Owner.Id,
                        ListOfPlayers = list,
                        Status = OperationStatus.Successfull,
                        Details = $"Room {request.RoomId} successfully started.",
                        ScoreBoard = ConvertScoreboardToSmallScoreboard(Scoreboard)
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] LoadingGame for user {request.RequesterId}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new LoadGameResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }
        
        private async void ContinueGame()
        {
            try
            {
                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    HandledRoom = data.Rooms.Get(HandledRoom.Id);

                    var puzzle = data.Puzzles.Get(HandledRoom.CurrentGame.PuzzleForGame.Id + 3); //vraca sledcu puzzluuuuu :D

                    data.Games.Remove(HandledRoom.CurrentGame);
                    data.Complete();

                    var game = new Game
                    {
                        PuzzleForGame = puzzle,
                        RoomForThisGame = HandledRoom
                    };

                    data.Games.Add(game);
                    data.Complete();

                    Log($"[SUCCESS] Room {HandledRoom.Id} successfully continued");

                    //await Task.Delay(5000);
                    NotifyAllToContinue();
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Continuing room {HandledRoom.Id}, Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);
                NotifyAllFailedContinue();
            }
        }

        private void NotifyAllFailedContinue()
        {
            string routingKey = RouteGenerator.RoomUpdates.Room.Set.FromEnum(RoomUpdateType.Deleted, HandledRoom.Id);
            Communicator.Publish<RoomsStateUpdate>(new RoomsStateUpdate()
            {
                RoomId = HandledRoom.Id,
                UpdateType = RoomUpdateType.Deleted,
                MaxPlayers = HandledRoom.MaxPlayers,
                Details = "Failed to continue room",
                NumberOfRounds = HandledRoom.NumberOfRounds,
                Status = OperationStatus.Successfull,
                Level = HandledRoom.Difficulty,
                IsPublic = HandledRoom.IsPublic,
                Creator = ConvertPlayerToSubDTOPlayer(HandledRoom.Owner),
                CommunicationKey = String.Empty
            }, routingKey);

            Log($"NOTIFY_ALL: {routingKey}");
        }

        private void NotifyAllToContinue()
        {
            string routingKey = RouteGenerator.RoomUpdates.Room.Set.FromEnum(RoomUpdateType.Continued, HandledRoom.Id);
            Communicator.Publish<RoomsStateUpdate>(new RoomsStateUpdate()
            {
                RoomId = HandledRoom.Id,
                UpdateType = RoomUpdateType.Continued,
                MaxPlayers = HandledRoom.MaxPlayers,
                Details = "RoomContinued",
                NumberOfRounds = HandledRoom.NumberOfRounds,
                Status = OperationStatus.Successfull,
                Level = HandledRoom.Difficulty,
                IsPublic = HandledRoom.IsPublic,
                Creator = ConvertPlayerToSubDTOPlayer(HandledRoom.Owner),
                CommunicationKey = String.Empty
            }, routingKey);

            Log($"NOTIFY_ALL: {routingKey}");
        }

        #endregion

        #region Utils

        private GameUpdate GenerateGameUpdate(Player currentPlayer, Move playedMove, GamePlayUpdateType updateType)
        {
            return new GameUpdate
            {
                Status = OperationStatus.Successfull,
                Details = "Move is played.",
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
            return new Scoreboard()
            {
                Scores = sb.Select(
                            x => new Tuple<DTOLibrary.SubDTOs.Player, int>(
                                    ConvertPlayerToSubDTOPlayer(x.Item1), x.Item2)).ToList()
            };
        }

        private bool EndOfTheRound()
        {
            return NumberOfSolvedPieces == NumberOfPieces;
        }

        private bool EndOfTheGame()
        {
            return CurrentRound == HandledRoom.NumberOfRounds + 1;
        }

        private void HandleMove(MakeAMoveRequest move)
        {
            if (move.RequesterId != CurrentPlayer.Id)
            {
                Log($"Failed to play move. Player {move.RequesterId} is not on a move", LogMessageType.Warning);
                return;
            }

            Move playedMove = move.MoveToPlay;
            playedMove.IsSuccessfull = (move.MoveToPlay.PositionFrom == move.MoveToPlay.PositionTo);
            playedMove.PlayedBy = ConvertPlayerToSubDTOPlayer(CurrentPlayer);

            if (playedMove.IsSuccessfull)
            {
                ++NumberOfSolvedPieces;
                var score = Scoreboard.FindIndex(x => x.Item1.Id == move.RequesterId);
                var newScore = new Tuple<Player, int>(Scoreboard[score].Item1, Scoreboard[score].Item2 + 5);
                Scoreboard.RemoveAt(score);
                Scoreboard.Insert(score, newScore);
            }

            GameUpdate gameUpdate;

            if (EndOfTheRound())
            {
                ++CurrentRound;
                gameUpdate = GenerateGameUpdate(CurrentPlayer, playedMove, EndOfTheGame() ? 
                    GamePlayUpdateType.GameOver : GamePlayUpdateType.RoundOver);
            }
            else
            {
                CurrentPlayer = playedMove.IsSuccessfull ? CurrentPlayer : NextPlayer();
                gameUpdate = GenerateGameUpdate(CurrentPlayer, playedMove, GamePlayUpdateType.Playing);
            }

            ////DEBUG ONLY
            //gameUpdate = GenerateGameUpdate(CurrentPlayer, playedMove, GamePlayUpdateType.GameOver);
            //NumberOfSolvedPieces = NumberOfPieces;

            Log($"Player {move.RequesterId} played move.");


            if (!EndOfTheGame() && EndOfTheRound())
            {
                ContinueGame();
            }

            NotifyAll(gameUpdate);
        }

        private Player NextPlayer()
        {
            int indexOfNext = (HandledRoom.ListOfPlayers.IndexOf(CurrentPlayer) + 1) % HandledRoom.ListOfPlayers.Count;
            return HandledRoom.ListOfPlayers[indexOfNext];
        }

        #endregion


    }
}
