namespace DEBUG_PostROman
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.groupBoxRooms = new System.Windows.Forms.GroupBox();
            this.dataGridViewRooms = new System.Windows.Forms.DataGridView();
            this.groupBoxInRoom = new System.Windows.Forms.GroupBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.groupBoxCreateRoom = new System.Windows.Forms.GroupBox();
            this.labelRoomID = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCreateRoom = new System.Windows.Forms.Button();
            this.buttonCancelRoom = new System.Windows.Forms.Button();
            this.groupBoxJoinRoom = new System.Windows.Forms.GroupBox();
            this.checkBoxReady = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonJoin = new System.Windows.Forms.Button();
            this.buttonLeaveRoom = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonRefreshRooms = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBoxRooms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRooms)).BeginInit();
            this.groupBoxInRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.groupBoxCreateRoom.SuspendLayout();
            this.groupBoxJoinRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxPassword);
            this.groupBox1.Controls.Add(this.btnLogin);
            this.groupBox1.Controls.Add(this.btnLogout);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxUsername);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 108);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(67, 45);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(206, 20);
            this.textBoxPassword.TabIndex = 5;
            this.textBoxPassword.Text = "666";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(67, 71);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Enabled = false;
            this.btnLogout.Location = new System.Drawing.Point(198, 71);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Username";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(67, 19);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(206, 20);
            this.textBoxUsername.TabIndex = 0;
            this.textBoxUsername.Text = "postroman";
            // 
            // groupBoxRooms
            // 
            this.groupBoxRooms.Controls.Add(this.buttonRefreshRooms);
            this.groupBoxRooms.Controls.Add(this.dataGridViewRooms);
            this.groupBoxRooms.Enabled = false;
            this.groupBoxRooms.Location = new System.Drawing.Point(298, 12);
            this.groupBoxRooms.Name = "groupBoxRooms";
            this.groupBoxRooms.Size = new System.Drawing.Size(945, 299);
            this.groupBoxRooms.TabIndex = 1;
            this.groupBoxRooms.TabStop = false;
            this.groupBoxRooms.Text = "MainScreen - Rooms";
            // 
            // dataGridViewRooms
            // 
            this.dataGridViewRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRooms.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewRooms.Name = "dataGridViewRooms";
            this.dataGridViewRooms.Size = new System.Drawing.Size(852, 251);
            this.dataGridViewRooms.TabIndex = 0;
            // 
            // groupBoxInRoom
            // 
            this.groupBoxInRoom.Controls.Add(this.dataGridView2);
            this.groupBoxInRoom.Controls.Add(this.propertyGrid1);
            this.groupBoxInRoom.Enabled = false;
            this.groupBoxInRoom.Location = new System.Drawing.Point(298, 317);
            this.groupBoxInRoom.Name = "groupBoxInRoom";
            this.groupBoxInRoom.Size = new System.Drawing.Size(951, 360);
            this.groupBoxInRoom.TabIndex = 2;
            this.groupBoxInRoom.TabStop = false;
            this.groupBoxInRoom.Text = "InRoom";
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(501, 20);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(444, 334);
            this.dataGridView2.TabIndex = 1;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(6, 19);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(489, 335);
            this.propertyGrid1.TabIndex = 0;
            // 
            // groupBoxCreateRoom
            // 
            this.groupBoxCreateRoom.Controls.Add(this.labelRoomID);
            this.groupBoxCreateRoom.Controls.Add(this.label3);
            this.groupBoxCreateRoom.Controls.Add(this.buttonCreateRoom);
            this.groupBoxCreateRoom.Controls.Add(this.buttonCancelRoom);
            this.groupBoxCreateRoom.Enabled = false;
            this.groupBoxCreateRoom.Location = new System.Drawing.Point(12, 126);
            this.groupBoxCreateRoom.Name = "groupBoxCreateRoom";
            this.groupBoxCreateRoom.Size = new System.Drawing.Size(280, 108);
            this.groupBoxCreateRoom.TabIndex = 3;
            this.groupBoxCreateRoom.TabStop = false;
            this.groupBoxCreateRoom.Text = "Create Room";
            // 
            // labelRoomID
            // 
            this.labelRoomID.AutoSize = true;
            this.labelRoomID.Location = new System.Drawing.Point(63, 74);
            this.labelRoomID.Name = "labelRoomID";
            this.labelRoomID.Size = new System.Drawing.Size(0, 13);
            this.labelRoomID.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "RoomID:";
            // 
            // buttonCreateRoom
            // 
            this.buttonCreateRoom.Location = new System.Drawing.Point(11, 19);
            this.buttonCreateRoom.Name = "buttonCreateRoom";
            this.buttonCreateRoom.Size = new System.Drawing.Size(262, 23);
            this.buttonCreateRoom.TabIndex = 4;
            this.buttonCreateRoom.Text = "Create Room";
            this.buttonCreateRoom.UseVisualStyleBackColor = true;
            this.buttonCreateRoom.Click += new System.EventHandler(this.buttonCreateRoom_Click);
            // 
            // buttonCancelRoom
            // 
            this.buttonCancelRoom.Enabled = false;
            this.buttonCancelRoom.Location = new System.Drawing.Point(11, 48);
            this.buttonCancelRoom.Name = "buttonCancelRoom";
            this.buttonCancelRoom.Size = new System.Drawing.Size(262, 23);
            this.buttonCancelRoom.TabIndex = 3;
            this.buttonCancelRoom.Text = "Cancel Room";
            this.buttonCancelRoom.UseVisualStyleBackColor = true;
            this.buttonCancelRoom.Click += new System.EventHandler(this.buttonCancelRoom_Click);
            // 
            // groupBoxJoinRoom
            // 
            this.groupBoxJoinRoom.Controls.Add(this.checkBoxReady);
            this.groupBoxJoinRoom.Controls.Add(this.label4);
            this.groupBoxJoinRoom.Controls.Add(this.label5);
            this.groupBoxJoinRoom.Controls.Add(this.buttonJoin);
            this.groupBoxJoinRoom.Controls.Add(this.buttonLeaveRoom);
            this.groupBoxJoinRoom.Enabled = false;
            this.groupBoxJoinRoom.Location = new System.Drawing.Point(12, 240);
            this.groupBoxJoinRoom.Name = "groupBoxJoinRoom";
            this.groupBoxJoinRoom.Size = new System.Drawing.Size(280, 124);
            this.groupBoxJoinRoom.TabIndex = 4;
            this.groupBoxJoinRoom.TabStop = false;
            this.groupBoxJoinRoom.Text = "Create Room";
            // 
            // checkBoxReady
            // 
            this.checkBoxReady.AutoSize = true;
            this.checkBoxReady.Location = new System.Drawing.Point(13, 77);
            this.checkBoxReady.Name = "checkBoxReady";
            this.checkBoxReady.Size = new System.Drawing.Size(57, 17);
            this.checkBoxReady.TabIndex = 9;
            this.checkBoxReady.Text = "Ready";
            this.checkBoxReady.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(67, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "RoomID:";
            // 
            // buttonJoin
            // 
            this.buttonJoin.Location = new System.Drawing.Point(11, 19);
            this.buttonJoin.Name = "buttonJoin";
            this.buttonJoin.Size = new System.Drawing.Size(262, 23);
            this.buttonJoin.TabIndex = 4;
            this.buttonJoin.Text = "JoinRoom";
            this.buttonJoin.UseVisualStyleBackColor = true;
            // 
            // buttonLeaveRoom
            // 
            this.buttonLeaveRoom.Enabled = false;
            this.buttonLeaveRoom.Location = new System.Drawing.Point(12, 48);
            this.buttonLeaveRoom.Name = "buttonLeaveRoom";
            this.buttonLeaveRoom.Size = new System.Drawing.Size(262, 23);
            this.buttonLeaveRoom.TabIndex = 3;
            this.buttonLeaveRoom.Text = "LeaveRoom";
            this.buttonLeaveRoom.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(78, 380);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(121, 120);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // buttonRefreshRooms
            // 
            this.buttonRefreshRooms.Location = new System.Drawing.Point(864, 19);
            this.buttonRefreshRooms.Name = "buttonRefreshRooms";
            this.buttonRefreshRooms.Size = new System.Drawing.Size(75, 23);
            this.buttonRefreshRooms.TabIndex = 1;
            this.buttonRefreshRooms.Text = "Refresh";
            this.buttonRefreshRooms.UseVisualStyleBackColor = true;
            this.buttonRefreshRooms.Click += new System.EventHandler(this.buttonRefreshRooms_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(239)))), ((int)(((byte)(241)))));
            this.ClientSize = new System.Drawing.Size(1261, 698);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBoxJoinRoom);
            this.Controls.Add(this.groupBoxCreateRoom);
            this.Controls.Add(this.groupBoxInRoom);
            this.Controls.Add(this.groupBoxRooms);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormMain";
            this.Text = "PostROman";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxRooms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRooms)).EndInit();
            this.groupBoxInRoom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.groupBoxCreateRoom.ResumeLayout(false);
            this.groupBoxCreateRoom.PerformLayout();
            this.groupBoxJoinRoom.ResumeLayout(false);
            this.groupBoxJoinRoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.GroupBox groupBoxRooms;
        private System.Windows.Forms.DataGridView dataGridViewRooms;
        private System.Windows.Forms.GroupBox groupBoxInRoom;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.GroupBox groupBoxCreateRoom;
        private System.Windows.Forms.Label labelRoomID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCreateRoom;
        private System.Windows.Forms.Button buttonCancelRoom;
        private System.Windows.Forms.GroupBox groupBoxJoinRoom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonJoin;
        private System.Windows.Forms.Button buttonLeaveRoom;
        private System.Windows.Forms.CheckBox checkBoxReady;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonRefreshRooms;
    }
}

