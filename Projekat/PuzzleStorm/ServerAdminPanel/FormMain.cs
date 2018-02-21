using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using StormCommonData.Enums;
using StormCommonData.EventArgs;
using StormCommonData.Interfaces;

namespace ServerAdminPanel
{
    public partial class FormMain : Form
    {
        private readonly BindingList<KeyValuePair<string, IStormServer>> availableServers = new BindingList<KeyValuePair<string, IStormServer>>();
        private readonly BindingList<KeyValuePair<string, IStormServer>> runningServers = new BindingList<KeyValuePair<string, IStormServer>>();
        
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            availableServers.Add(new KeyValuePair<string, IStormServer>("Auth Server",  ServerAuth.ServerAuth.Instance));
            availableServers.Add(new KeyValuePair<string, IStormServer>("Lobby Server", ServerLobby.ServerLobby.Instance));
            availableServers.Add(new KeyValuePair<string, IStormServer>("Game Server", ServerGame.ServerGame.Instance));

            foreach (var item in availableServers)
                item.Value.NewLogMessage += OnNewServerLogMessage;

            listBoxAvailableServers.DisplayMember = "Key";
            listBoxAvailableServers.ValueMember = "Value";
            listBoxAvailableServers.DataSource = availableServers;

            listBoxRunningServers.DisplayMember = "Key";
            listBoxRunningServers.ValueMember = "Value";
            listBoxRunningServers.DataSource = runningServers;

            Dispatcher.CurrentDispatcher.InvokeAsync(() => buttonStartAll_Click(this, null));
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (listBoxAvailableServers.SelectedItems.Count == 0)
                return;

            DisableInput();

            var selectedItem = (KeyValuePair<string, IStormServer>) listBoxAvailableServers.SelectedItem;
            var selectedServer = selectedItem.Value;

            selectedServer.Start();
            
            runningServers.Add(selectedItem);
            availableServers.Remove(selectedItem);

            EnableInput();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (listBoxRunningServers.SelectedItems.Count == 0)
                return;

            DisableInput();

            var selectedItem = (KeyValuePair<string, IStormServer>)listBoxRunningServers.SelectedItem;
            var selectedServer = selectedItem.Value;

            selectedServer.Stop();

            availableServers.Add(selectedItem);
            runningServers.Remove(selectedItem);

            EnableInput();
        }

        
        private void OnNewServerLogMessage(object sender, LogMessageArgs logMessageArgs)
        {
            Color textColor;

            switch (logMessageArgs.Type)
            {
                case LogMessageType.Info:
                    textColor = Color.Black;
                    break;
                case LogMessageType.Warning:
                    textColor = Color.Coral;
                    break;
                case LogMessageType.Error:
                    textColor = Color.Crimson;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (txtBoxOutput.InvokeRequired)
                txtBoxOutput.Invoke(new Action(() =>
                {
                    OutputWriter(logMessageArgs.Message, textColor);
                }));
            else
                OutputWriter(logMessageArgs.Message, textColor);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var listItem in runningServers)
            {
                listItem.Value.Stop();
            }

            foreach (var listItem in availableServers)
            {
                listItem.Value.Dispose();
            }
        }

        private void OutputWriter(string text, Color color)
        {
            txtBoxOutput.SelectionStart = txtBoxOutput.TextLength;
            txtBoxOutput.SelectionLength = 0;

            txtBoxOutput.SelectionColor = color;
            txtBoxOutput.AppendText(text + Environment.NewLine);
            txtBoxOutput.SelectionColor = txtBoxOutput.ForeColor;
        }

        private void DisableInput()
        {
            panel1.Enabled = false;
        }

        private void EnableInput()
        {
            panel1.Enabled = true;
        }

        private void buttonClearOutput_Click(object sender, EventArgs e)
        {
            txtBoxOutput.Clear();
        }

        private void buttonStartAll_Click(object sender, EventArgs e)
        {
            DisableInput();

            foreach (var listItem in availableServers)
            {
                listItem.Value.Start();
                runningServers.Add(listItem);
            }
            availableServers.Clear();

            EnableInput();
        }

        private void buttonStopAll_Click(object sender, EventArgs e)
        {
            DisableInput();

            foreach (var listItem in runningServers)
            {
                listItem.Value.Stop();

                availableServers.Add(listItem);
            }
            runningServers.Clear();

            EnableInput();
        }
    }
}
