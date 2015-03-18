namespace UglyGCS
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.missionPanel = new System.Windows.Forms.GroupBox();
            this.currentMissionLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.missionStateLabel = new System.Windows.Forms.Label();
            this.startMissionComboBox = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.missionComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.missionHistoryListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.floatSendRadioButton = new System.Windows.Forms.RadioButton();
            this.intSendRadioButton = new System.Windows.Forms.RadioButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.recievedDataRichTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.sentDataRichTextBox = new System.Windows.Forms.RichTextBox();
            this.commandValueTextBox = new System.Windows.Forms.TextBox();
            this.sendCommandButton = new System.Windows.Forms.Button();
            this.commandComboBox = new System.Windows.Forms.ComboBox();
            this.recievedCommandsListView = new System.Windows.Forms.ListView();
            this.Command = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.cameraListComboBox = new System.Windows.Forms.ComboBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.rescanButton = new System.Windows.Forms.Button();
            this.portsComboBox = new System.Windows.Forms.ComboBox();
            this.imgDebugWindowButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.processingTimeLabel = new System.Windows.Forms.Label();
            this.timelapsTimer = new System.Windows.Forms.Timer(this.components);
            this.cameraControl1 = new UglyGCS.CameraControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.imgDebugWindowButton);
            this.splitContainer1.Panel2.Controls.Add(this.cameraControl1);
            this.splitContainer1.Size = new System.Drawing.Size(763, 443);
            this.splitContainer1.SplitterDistance = 293;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(293, 443);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.processingTimeLabel);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.missionPanel);
            this.tabPage1.Controls.Add(this.currentMissionLabel);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.missionStateLabel);
            this.tabPage1.Controls.Add(this.startMissionComboBox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.missionComboBox);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(285, 417);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Mission";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // missionPanel
            // 
            this.missionPanel.Location = new System.Drawing.Point(11, 113);
            this.missionPanel.Name = "missionPanel";
            this.missionPanel.Size = new System.Drawing.Size(268, 167);
            this.missionPanel.TabIndex = 10;
            this.missionPanel.TabStop = false;
            this.missionPanel.Text = "Current mission parameters";
            // 
            // currentMissionLabel
            // 
            this.currentMissionLabel.AutoSize = true;
            this.currentMissionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.currentMissionLabel.Location = new System.Drawing.Point(110, 64);
            this.currentMissionLabel.Name = "currentMissionLabel";
            this.currentMissionLabel.Size = new System.Drawing.Size(35, 15);
            this.currentMissionLabel.TabIndex = 9;
            this.currentMissionLabel.Text = "none";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(8, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Current mission:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(8, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Current state:";
            // 
            // missionStateLabel
            // 
            this.missionStateLabel.AutoSize = true;
            this.missionStateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.missionStateLabel.ForeColor = System.Drawing.Color.Red;
            this.missionStateLabel.Location = new System.Drawing.Point(93, 40);
            this.missionStateLabel.Name = "missionStateLabel";
            this.missionStateLabel.Size = new System.Drawing.Size(139, 15);
            this.missionStateLabel.TabIndex = 0;
            this.missionStateLabel.Text = "Mission not selected";
            // 
            // startMissionComboBox
            // 
            this.startMissionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startMissionComboBox.Location = new System.Drawing.Point(207, 9);
            this.startMissionComboBox.Name = "startMissionComboBox";
            this.startMissionComboBox.Size = new System.Drawing.Size(75, 23);
            this.startMissionComboBox.TabIndex = 5;
            this.startMissionComboBox.Text = "START";
            this.startMissionComboBox.UseVisualStyleBackColor = true;
            this.startMissionComboBox.Click += new System.EventHandler(this.startMissionComboBox_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Select mission:";
            // 
            // missionComboBox
            // 
            this.missionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.missionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.missionComboBox.FormattingEnabled = true;
            this.missionComboBox.Location = new System.Drawing.Point(91, 9);
            this.missionComboBox.Name = "missionComboBox";
            this.missionComboBox.Size = new System.Drawing.Size(110, 21);
            this.missionComboBox.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.missionHistoryListView);
            this.groupBox3.Location = new System.Drawing.Point(3, 286);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(276, 128);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Mission history";
            // 
            // missionHistoryListView
            // 
            this.missionHistoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.missionHistoryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.missionHistoryListView.Location = new System.Drawing.Point(3, 16);
            this.missionHistoryListView.Name = "missionHistoryListView";
            this.missionHistoryListView.Size = new System.Drawing.Size(270, 109);
            this.missionHistoryListView.TabIndex = 2;
            this.missionHistoryListView.UseCompatibleStateImageBehavior = false;
            this.missionHistoryListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Misja";
            this.columnHeader1.Width = 85;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Info";
            this.columnHeader2.Width = 178;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.floatSendRadioButton);
            this.tabPage2.Controls.Add(this.intSendRadioButton);
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Controls.Add(this.commandValueTextBox);
            this.tabPage2.Controls.Add(this.sendCommandButton);
            this.tabPage2.Controls.Add(this.commandComboBox);
            this.tabPage2.Controls.Add(this.recievedCommandsListView);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.cameraListComboBox);
            this.tabPage2.Controls.Add(this.connectButton);
            this.tabPage2.Controls.Add(this.rescanButton);
            this.tabPage2.Controls.Add(this.portsComboBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(285, 417);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Devices";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // floatSendRadioButton
            // 
            this.floatSendRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.floatSendRadioButton.AutoSize = true;
            this.floatSendRadioButton.Checked = true;
            this.floatSendRadioButton.Location = new System.Drawing.Point(159, 95);
            this.floatSendRadioButton.Name = "floatSendRadioButton";
            this.floatSendRadioButton.Size = new System.Drawing.Size(59, 17);
            this.floatSendRadioButton.TabIndex = 11;
            this.floatSendRadioButton.TabStop = true;
            this.floatSendRadioButton.Text = "FLOAT";
            this.floatSendRadioButton.UseVisualStyleBackColor = true;
            // 
            // intSendRadioButton
            // 
            this.intSendRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.intSendRadioButton.AutoSize = true;
            this.intSendRadioButton.Location = new System.Drawing.Point(159, 69);
            this.intSendRadioButton.Name = "intSendRadioButton";
            this.intSendRadioButton.Size = new System.Drawing.Size(43, 17);
            this.intSendRadioButton.TabIndex = 10;
            this.intSendRadioButton.Text = "INT";
            this.intSendRadioButton.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 244);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(279, 170);
            this.splitContainer2.SplitterDistance = 128;
            this.splitContainer2.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.recievedDataRichTextBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(128, 170);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recieved data";
            // 
            // recievedDataRichTextBox
            // 
            this.recievedDataRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recievedDataRichTextBox.Location = new System.Drawing.Point(3, 16);
            this.recievedDataRichTextBox.Name = "recievedDataRichTextBox";
            this.recievedDataRichTextBox.ReadOnly = true;
            this.recievedDataRichTextBox.Size = new System.Drawing.Size(122, 151);
            this.recievedDataRichTextBox.TabIndex = 0;
            this.recievedDataRichTextBox.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.sentDataRichTextBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(147, 170);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sent data";
            // 
            // sentDataRichTextBox
            // 
            this.sentDataRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sentDataRichTextBox.Location = new System.Drawing.Point(3, 16);
            this.sentDataRichTextBox.Name = "sentDataRichTextBox";
            this.sentDataRichTextBox.ReadOnly = true;
            this.sentDataRichTextBox.Size = new System.Drawing.Size(141, 151);
            this.sentDataRichTextBox.TabIndex = 0;
            this.sentDataRichTextBox.Text = "";
            // 
            // commandValueTextBox
            // 
            this.commandValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commandValueTextBox.Location = new System.Drawing.Point(6, 95);
            this.commandValueTextBox.Name = "commandValueTextBox";
            this.commandValueTextBox.Size = new System.Drawing.Size(147, 20);
            this.commandValueTextBox.TabIndex = 8;
            // 
            // sendCommandButton
            // 
            this.sendCommandButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sendCommandButton.Location = new System.Drawing.Point(222, 66);
            this.sendCommandButton.Name = "sendCommandButton";
            this.sendCommandButton.Size = new System.Drawing.Size(54, 46);
            this.sendCommandButton.TabIndex = 7;
            this.sendCommandButton.Text = "Send";
            this.sendCommandButton.UseVisualStyleBackColor = true;
            this.sendCommandButton.Click += new System.EventHandler(this.sendCommandButton_Click);
            // 
            // commandComboBox
            // 
            this.commandComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.commandComboBox.FormattingEnabled = true;
            this.commandComboBox.Location = new System.Drawing.Point(6, 68);
            this.commandComboBox.Name = "commandComboBox";
            this.commandComboBox.Size = new System.Drawing.Size(147, 21);
            this.commandComboBox.TabIndex = 6;
            // 
            // recievedCommandsListView
            // 
            this.recievedCommandsListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.recievedCommandsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Command,
            this.Value});
            this.recievedCommandsListView.Location = new System.Drawing.Point(6, 121);
            this.recievedCommandsListView.Name = "recievedCommandsListView";
            this.recievedCommandsListView.Size = new System.Drawing.Size(273, 117);
            this.recievedCommandsListView.TabIndex = 5;
            this.recievedCommandsListView.UseCompatibleStateImageBehavior = false;
            this.recievedCommandsListView.View = System.Windows.Forms.View.Details;
            // 
            // Command
            // 
            this.Command.Text = "Command";
            this.Command.Width = 153;
            // 
            // Value
            // 
            this.Value.Text = "Value";
            this.Value.Width = 71;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(159, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Open camera";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cameraListComboBox
            // 
            this.cameraListComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cameraListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cameraListComboBox.FormattingEnabled = true;
            this.cameraListComboBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
            this.cameraListComboBox.Location = new System.Drawing.Point(6, 9);
            this.cameraListComboBox.Name = "cameraListComboBox";
            this.cameraListComboBox.Size = new System.Drawing.Size(147, 21);
            this.cameraListComboBox.TabIndex = 3;
            // 
            // connectButton
            // 
            this.connectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.connectButton.Location = new System.Drawing.Point(159, 38);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(57, 23);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // rescanButton
            // 
            this.rescanButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rescanButton.Location = new System.Drawing.Point(222, 38);
            this.rescanButton.Name = "rescanButton";
            this.rescanButton.Size = new System.Drawing.Size(54, 23);
            this.rescanButton.TabIndex = 1;
            this.rescanButton.Text = "Rescan";
            this.rescanButton.UseVisualStyleBackColor = true;
            // 
            // portsComboBox
            // 
            this.portsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.portsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.portsComboBox.FormattingEnabled = true;
            this.portsComboBox.Location = new System.Drawing.Point(6, 36);
            this.portsComboBox.Name = "portsComboBox";
            this.portsComboBox.Size = new System.Drawing.Size(147, 21);
            this.portsComboBox.TabIndex = 0;
            // 
            // imgDebugWindowButton
            // 
            this.imgDebugWindowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imgDebugWindowButton.Location = new System.Drawing.Point(319, 0);
            this.imgDebugWindowButton.Name = "imgDebugWindowButton";
            this.imgDebugWindowButton.Size = new System.Drawing.Size(147, 23);
            this.imgDebugWindowButton.TabIndex = 1;
            this.imgDebugWindowButton.Text = "Image Debug Window";
            this.imgDebugWindowButton.UseVisualStyleBackColor = true;
            this.imgDebugWindowButton.Click += new System.EventHandler(this.imgDebugWindowButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Current frame processing time:";
            // 
            // processingTimeLabel
            // 
            this.processingTimeLabel.AutoSize = true;
            this.processingTimeLabel.Location = new System.Drawing.Point(163, 88);
            this.processingTimeLabel.Name = "processingTimeLabel";
            this.processingTimeLabel.Size = new System.Drawing.Size(13, 13);
            this.processingTimeLabel.TabIndex = 11;
            this.processingTimeLabel.Text = "0";
            // 
            // timelapsTimer
            // 
            this.timelapsTimer.Enabled = true;
            this.timelapsTimer.Tick += new System.EventHandler(this.timelapsTimer_Tick);
            // 
            // cameraControl1
            // 
            this.cameraControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraControl1.IsRecording = false;
            this.cameraControl1.Location = new System.Drawing.Point(0, 0);
            this.cameraControl1.Name = "cameraControl1";
            this.cameraControl1.Size = new System.Drawing.Size(466, 443);
            this.cameraControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 443);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "UglyGCS";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button rescanButton;
        private System.Windows.Forms.ComboBox portsComboBox;
        private CameraControl cameraControl1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cameraListComboBox;
        private System.Windows.Forms.ListView recievedCommandsListView;
        private System.Windows.Forms.ColumnHeader Command;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.TextBox commandValueTextBox;
        private System.Windows.Forms.Button sendCommandButton;
        private System.Windows.Forms.ComboBox commandComboBox;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox recievedDataRichTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox sentDataRichTextBox;
        private System.Windows.Forms.Button imgDebugWindowButton;
        private System.Windows.Forms.Button startMissionComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox missionComboBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView missionHistoryListView;
        private System.Windows.Forms.RadioButton floatSendRadioButton;
        private System.Windows.Forms.RadioButton intSendRadioButton;
        private System.Windows.Forms.Label missionStateLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label currentMissionLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox missionPanel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label processingTimeLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer timelapsTimer;
    }
}

