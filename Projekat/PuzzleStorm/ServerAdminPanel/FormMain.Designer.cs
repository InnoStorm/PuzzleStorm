namespace ServerAdminPanel
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
            this.txtBoxOutput = new System.Windows.Forms.RichTextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.listBoxAvailableServers = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClearOutput = new System.Windows.Forms.Button();
            this.buttonStopAll = new System.Windows.Forms.Button();
            this.buttonStartAll = new System.Windows.Forms.Button();
            this.listBoxRunningServers = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtBoxOutput
            // 
            this.txtBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxOutput.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtBoxOutput.HideSelection = false;
            this.txtBoxOutput.Location = new System.Drawing.Point(3, 21);
            this.txtBoxOutput.Name = "txtBoxOutput";
            this.txtBoxOutput.ReadOnly = true;
            this.txtBoxOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtBoxOutput.Size = new System.Drawing.Size(617, 463);
            this.txtBoxOutput.TabIndex = 0;
            this.txtBoxOutput.Text = "";
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStart.Location = new System.Drawing.Point(6, 400);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(109, 26);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "START";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.Location = new System.Drawing.Point(121, 400);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(103, 26);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "STOP";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // listBoxAvailableServers
            // 
            this.listBoxAvailableServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAvailableServers.FormattingEnabled = true;
            this.listBoxAvailableServers.Location = new System.Drawing.Point(6, 21);
            this.listBoxAvailableServers.Name = "listBoxAvailableServers";
            this.listBoxAvailableServers.Size = new System.Drawing.Size(218, 173);
            this.listBoxAvailableServers.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Available Servers";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.buttonClearOutput);
            this.panel1.Controls.Add(this.buttonStopAll);
            this.panel1.Controls.Add(this.buttonStartAll);
            this.panel1.Controls.Add(this.listBoxRunningServers);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.listBoxAvailableServers);
            this.panel1.Controls.Add(this.buttonStop);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonStart);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(231, 487);
            this.panel1.TabIndex = 5;
            // 
            // buttonClearOutput
            // 
            this.buttonClearOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClearOutput.Location = new System.Drawing.Point(6, 461);
            this.buttonClearOutput.Name = "buttonClearOutput";
            this.buttonClearOutput.Size = new System.Drawing.Size(218, 23);
            this.buttonClearOutput.TabIndex = 9;
            this.buttonClearOutput.Text = "CLEAR";
            this.buttonClearOutput.UseVisualStyleBackColor = true;
            this.buttonClearOutput.Click += new System.EventHandler(this.buttonClearOutput_Click);
            // 
            // buttonStopAll
            // 
            this.buttonStopAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStopAll.Location = new System.Drawing.Point(119, 432);
            this.buttonStopAll.Name = "buttonStopAll";
            this.buttonStopAll.Size = new System.Drawing.Size(105, 26);
            this.buttonStopAll.TabIndex = 8;
            this.buttonStopAll.Text = "STOP ALL";
            this.buttonStopAll.UseVisualStyleBackColor = true;
            this.buttonStopAll.Click += new System.EventHandler(this.buttonStopAll_Click);
            // 
            // buttonStartAll
            // 
            this.buttonStartAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStartAll.Location = new System.Drawing.Point(6, 432);
            this.buttonStartAll.Name = "buttonStartAll";
            this.buttonStartAll.Size = new System.Drawing.Size(109, 26);
            this.buttonStartAll.TabIndex = 7;
            this.buttonStartAll.Text = "START ALL";
            this.buttonStartAll.UseVisualStyleBackColor = true;
            this.buttonStartAll.Click += new System.EventHandler(this.buttonStartAll_Click);
            // 
            // listBoxRunningServers
            // 
            this.listBoxRunningServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxRunningServers.FormattingEnabled = true;
            this.listBoxRunningServers.Location = new System.Drawing.Point(6, 213);
            this.listBoxRunningServers.Name = "listBoxRunningServers";
            this.listBoxRunningServers.Size = new System.Drawing.Size(218, 186);
            this.listBoxRunningServers.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Running Servers";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtBoxOutput);
            this.panel2.Location = new System.Drawing.Point(249, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(623, 487);
            this.panel2.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Output";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 511);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(900, 550);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PuzzleStorm Admin Panel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtBoxOutput;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.ListBox listBoxAvailableServers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxRunningServers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonClearOutput;
        private System.Windows.Forms.Button buttonStopAll;
        private System.Windows.Forms.Button buttonStartAll;
    }
}

