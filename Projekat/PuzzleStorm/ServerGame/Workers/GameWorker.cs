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

        public MakeAMoveResponse MakeAMove(MakeAMoveRequest request)
        {
            try
            {
                using (var data = WorkersUnitOfWork)
                {
                    var player = data.Players.Get(request.RequesterId);

                    MakeAMoveResponse response = new MakeAMoveResponse
                    {
                        Status = OperationStatus.Successfull,
                        Details = "Successfull"
                    };

                    if (request.SelectedPartNumber != request.TablePlaceNumber)
                        response.CurrentPlayerId = NextPlayer(request.RoomId, request.RequesterId);
                    else
                    {
                        response.CurrentPlayerId = request.RequesterId;
                        //player.Score += 5;
                        //AddPointsToPlayer(player, request.RoomId);
                    }
                    //response.ScoreBoard = 

                    Log($"[SUCCESS] Making move. Requester: {request.RequesterId}");
                    return response;
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Making move. Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);

                return new MakeAMoveResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }
    }
}
