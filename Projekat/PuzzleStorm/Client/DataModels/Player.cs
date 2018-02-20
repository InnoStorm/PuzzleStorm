using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Helpers.Communication;
using Communicator;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;

namespace Client {
    public sealed class Player {

        private static Player instance = null;

        public int Id { get; set; }

        public string UserName { get; set; }

        public int RoomId { get; set; }

        public bool Creator { get; set; } = false; // kad napravi sobu setuje mu se na true

        public Game InGame { get; set; } = new Game();

        public string CommKey { get; set; }

        public bool OnTheMove { get; set; } = false;

        private Player() {
            
        }

        public static Player Instance {
            get {
                if (instance == null) {
                    instance = new Player();
                }

                return instance;
            }
        }

        public void Clean()
        {

            if (Player.Instance.Creator) //brisi sobu
                DeleteRoomAsync();
            else if (Player.Instance.RoomId > 0) //ostao u neku sobu
                DisconnectAsync();

            if (Player.Instance.Id > 0) //ostao ulogovan
                SignOutAsync();
        }   

        public async void SignOutAsync() {
            SignOutRequest request = new SignOutRequest() {
                RequesterId = Player.Instance.Id
            };

            await ClientUtils.PerformRequestAsync(API.Instance.SignOutAsync, request,
                null);
        }

        public async void DeleteRoomAsync()
        {
            CancelRoomRequest request = new CancelRoomRequest() {
                RequesterId = Player.Instance.Id,
                RoomId = Player.Instance.RoomId
            };

            await ClientUtils.PerformRequestAsync(API.Instance.CancelRoomAsync, request,
                null);
        }

        public async void DisconnectAsync()
        {
            LeaveRoomRequest request = new LeaveRoomRequest() {
                RequesterId = Player.Instance.Id,
                RoomId = Player.Instance.RoomId
            };

            await ClientUtils.PerformRequestAsync(API.Instance.LeaveRoomAsync, request,
                null);
        }
    }
}
