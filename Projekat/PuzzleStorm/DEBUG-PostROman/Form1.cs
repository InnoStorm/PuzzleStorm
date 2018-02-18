using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
using Communicator;
using DTOLibrary.Broadcasts;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using StormCommonData;
using StormCommonData.Enums;
using StormCommonData.Events;

namespace DEBUG_PostROman
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }


        private bool CheckError(Response response)
        {
            if (response.Status == OperationStatus.Failed)
                ShowError(response.Details);

            return response.Status == OperationStatus.Failed;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetFormState(bool enabledState)
        {
            groupBoxCreateRoom.Enabled = enabledState;
            groupBoxInRoom.Enabled = enabledState;
            groupBoxJoinRoom.Enabled = enabledState;
            groupBoxRooms.Enabled = enabledState;
        }


        private async void InitRoomList()
        {
            var request = new GetAllRoomsRequest()
            {
                RequesterId = Utils.LoginData.PlayerId
            };

            var response = await API.Instance.GetAllRoomsAsync(request);
            if (CheckError(response)) return;

            Utils.AvailableRooms = new BindingList<RoomInfo>(response.List);
            dataGridViewRooms.DataSource = Utils.AvailableRooms;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            API.Instance.Dispose();
            
        }
        
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            
            var request = new LoginRequest()
            {
                Username = textBoxUsername.Text,
                Password = textBoxPassword.Text
            };

            LoginResponse response = await API.Instance.LoginAsync(request);
            if (CheckError(response)) return;
            
            Utils.LoginData = response;
            btnLogin.Enabled = false;
            btnLogout.Enabled = true;
            SetFormState(true);

            InitRoomList();

            API.Instance.RoomChanged += OnRoomChangeHandler;
            API.Instance.SubscribeRoomChanges(
                Utils.LoginData.PlayerId.ToString(),
                RouteGenerator.RoomUpdates.Room.Filter.All()
                );

        }

        private void OnRoomChangeHandler(object sender, StormEventArgs<RoomsStateUpdate> stormEventArgs)
        {
            RoomsStateUpdate update = stormEventArgs.Data;

            switch (update.UpdateType)
            {
                case RoomUpdateType.Created:
                case RoomUpdateType.BecameAvailable:
                    Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                    {
                        Utils.AvailableRooms.Add(new RoomInfo()
                        {
                            RoomId = update.RoomId,
                            MaxPlayers = update.MaxPlayers,
                            CreatorUsername = update.Creator.Username,
                            IsPublic = update.IsPublic,
                            NumberOfRounds = update.NumberOfRounds,
                            Difficulty = update.Level
                        });
                        dataGridViewRooms.DataSource = Utils.AvailableRooms;
                    });
                    break;
                case RoomUpdateType.Modified:
                    break;
                case RoomUpdateType.Deleted:
                case RoomUpdateType.Started:
                case RoomUpdateType.Filled:
                    Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                        Utils.AvailableRooms.Remove(Utils.AvailableRooms.Single(x => x.RoomId == update.RoomId)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                this.Text = $"New Change detected: {update.UpdateType.ToString()}.{update.RoomId}");
        }

        private async void btnLogout_Click(object sender, EventArgs e)
        {

            var request = new SignOutRequest()
            {
                RequesterId = Utils.LoginData.PlayerId
            };

            var response = await API.Instance.SignOutAsync(request);
            if (CheckError(response)) return;

            btnLogin.Enabled = true;
            btnLogout.Enabled = false;
            SetFormState(false);

        }

        private async void buttonCreateRoom_Click(object sender, EventArgs e)
        {
            var request = new CreateRoomRequest()
            {
                RequesterId = Utils.LoginData.PlayerId,
                Difficulty = PuzzleDifficulty.Easy,
                MaxPlayers = 4,
                NumberOfRounds = 4
            };

            var response = await API.Instance.CreateRoomAsync(request);
            if (CheckError(response)) return;

            Utils.CreateRoomData = response;
            labelRoomID.Text = response.RoomId.ToString();

            buttonCreateRoom.Enabled = false;
            buttonCancelRoom.Enabled = true;
        }

        private async void buttonCancelRoom_Click(object sender, EventArgs e)
        {
            var request = new CancelRoomRequest()
            {
                RequesterId = Utils.LoginData.PlayerId,
                RoomId = Utils.CreateRoomData.RoomId
            };

            var response = await API.Instance.CancelRoomAsync(request);
            if (CheckError(response)) return;

            Utils.CreateRoomData = null;
            labelRoomID.Text = string.Empty;

            buttonCreateRoom.Enabled = true;
            buttonCancelRoom.Enabled = false;
        }

        private void buttonRefreshRooms_Click(object sender, EventArgs e)
        {
            buttonRefreshRooms.Enabled = false;

            InitRoomList();

            buttonRefreshRooms.Enabled = true;
        }
    }
}
