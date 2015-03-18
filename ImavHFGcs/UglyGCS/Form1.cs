using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using HighFlyers;
using UglyGCS.Mission;
using UglyGCS.Protocol;

namespace UglyGCS
{
    public partial class Form1 : Form
    {
        private readonly HashSet<int> pressedKeys = new HashSet<int>(); 
        private RS232 port;

        readonly ProtoBuffer readBuffer = new ProtoBuffer();
        private readonly KeyHandler keyHandler;
        readonly MissionManager missionManager = new MissionManager();

        public Form1()
        {
            InitializeComponent();

            readBuffer.FrameReady += readBuffer_FrameReady;
            cameraListComboBox.SelectedIndex = 0;
            portsComboBox.DataSource = RS232.ListOfAvailablePorts;
            if (portsComboBox.Items.Count > 0) portsComboBox.SelectedIndex = 0;
            commandComboBox.DataSource = Enum.GetNames(typeof(_FrameSrame));
            if (commandComboBox.Items.Count > 0) commandComboBox.SelectedIndex = 0;

            keyHandler = new KeyHandler(SendCommand);

            missionComboBox.DataSource = missionManager.GetMissions();

            cameraControl1.AlgorithmFinished += (sender, args) =>
                {
                    const string statusMessage = "Mission finished!!";
                    missionStateLabel.Text = statusMessage;
                    missionStateLabel.ForeColor = Color.Green;
                    var item = new ListViewItem(args.Mission.Title);
                    item.SubItems.Add(statusMessage);

                    missionHistoryListView.Items.Insert(0, item);
                };
        }

        private void SendCommand(int key, bool isKeyDown)
        {
            byte[] sent = SendCommandToPort(keyHandler.GetKeyCommand(isKeyDown, key),
                                            keyHandler.GetKeyValue(key));
            UpdateRichTextBox(sent, sentDataRichTextBox);
        }

        private byte[] SendCommandToPort(_FrameSrame command, int value)
        {
            var proto = new Proto
            {
                Command = command,
                ValueInt32 = value
            };

            return SendCommandToPort(proto);
        }

        private byte[] SendCommandToPort(_FrameSrame command, float value)
        {
            var proto = new Proto
                {
                    Command = command,
                    ValueFloat = value
                };

            return SendCommandToPort(proto);
        }


        private byte[] SendCommandToPort(Proto proto)
        {
            if (!IsPortOpened())
                return null;

            ProtoBuffer buffer = proto.ToBuffer();
            byte[] sent = buffer.PrepareToSend();
            port.SendData(sent);
            return sent;
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            const int wmKeydown = 0x100;
            const int wmKeyup = 0x101;

            if (m.Msg == wmKeydown || m.Msg == wmKeyup)
            {
                if (!pressedKeys.Contains((int)m.WParam) || m.Msg != wmKeydown)
                {
                    var key = (char) m.WParam;
                    key = Char.ToLower(key);
                    
                    keyHandler.DoMethod(key, m.Msg == wmKeydown);
                    if (m.Msg == wmKeydown)
                        pressedKeys.Add((int)m.WParam);
                    else
                        pressedKeys.Remove((int)m.WParam);
                }
            }

            return base.ProcessKeyPreview(ref m);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!cameraControl1.IsStarted)
                {
                    cameraControl1.Start(cameraListComboBox.SelectedIndex);
                    button1.Text = @"Close camera";
                }
                else
                {
                    cameraControl1.Stop();
                    button1.Text = @"Open camera";
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage(ex);
            }
        }

        private void ExceptionMessage(Exception exception)
        {
            MessageBox.Show(@"Exception occured: " + exception.Message);
        }
        
        #region Serial port
        
        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsPortOpened())
                {
                    port = new RS232(portsComboBox.SelectedItem.ToString(), 57600, 8);
                    port.OpenPort();
                    port.DataReceived += port_DataReceived;

                    connectButton.Text = @"Disconnect";
                }
                else
                {
                    port.Stop = true;

                    port.ClosePort();
                    connectButton.Text = @"Connect";
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage(ex);
            }
        }

        private bool IsPortOpened()
        {
            return port != null && port.IsOpen;
        }
        private void port_DataReceived(byte[] rcvd)
        {
            try
            {
                foreach (byte b in rcvd)
                {
                    readBuffer.Append(b);
                }

                recievedDataRichTextBox.Invoke(
                    new MethodInvoker(() => UpdateRichTextBox(rcvd, recievedDataRichTextBox)));
            }
            catch (Exception exc)
            {
                ExceptionMessage(exc);
            }
        }

        #endregion

        void readBuffer_FrameReady(object sender, ProtoEventArgs e)
        {
            Proto proto = e.Proto;
            recievedCommandsListView.Invoke(new MethodInvoker(
                                                () =>
                                                    {
                                                        var item = new ListViewItem(proto.Command.ToString());
                                                        item.SubItems.Add(
                                                            proto.ValueFloat.ToString(CultureInfo.InvariantCulture));
                                                        
                                                        recievedCommandsListView.Items.Insert(0, item);
                                                        if (recievedCommandsListView.Items.Count > 5)
                                                            recievedCommandsListView.Items.RemoveAt(
                                                                recievedCommandsListView.Items.Count - 1);
                                                    }));
            
        }

        private void sendCommandButton_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] sent = floatSendRadioButton.Checked
                                  ? SendCommandToPort(
                                      (_FrameSrame)
                                      Enum.Parse(typeof (_FrameSrame), commandComboBox.SelectedItem.ToString()),
                                      float.Parse(commandValueTextBox.Text.Replace('.', ',')))
                                  : SendCommandToPort(
                                      (_FrameSrame)
                                      Enum.Parse(typeof (_FrameSrame), commandComboBox.SelectedItem.ToString()),
                                      int.Parse(commandValueTextBox.Text));

                UpdateRichTextBox(sent, sentDataRichTextBox);

            }
            catch (Exception exc)
            {
                ExceptionMessage(exc);
            }
        }

        private void UpdateRichTextBox(byte[] bytes, RichTextBox richTextBox)
        {
            if (bytes == null)
                return;
            
            richTextBox.AppendText(GetHexView(bytes));
            richTextBox.SelectionStart = richTextBox.TextLength;
            richTextBox.ScrollToCaret();

            if (richTextBox.TextLength > 10000)
                richTextBox.Clear();
        }

        private string GetHexView(byte[] bytes)
        {
            string displayStr = StringParser.ByteToDisplay(bytes, DataFormat.Hex).Replace("{x", "");
            return displayStr.Replace('}', ' ');
        }

        private void imgDebugWindowButton_Click(object sender, EventArgs e)
        {
            var customForm = new Form();
            var pbox = new PictureBox();
            customForm.Controls.Add(pbox);
            pbox.Dock= DockStyle.Fill;
            customForm.ClientSize = new Size(640, 480);
            cameraControl1.SetPictureBox(pbox);
            customForm.Show();
        }

        private void startMissionComboBox_Click(object sender, EventArgs e)
        {
            var mission = missionComboBox.SelectedItem as Mission.Mission;
            if (mission == null) return;

            cameraControl1.SetMission(mission);
            mission.MissionControl.Dock = DockStyle.Fill;
            
            missionPanel.Controls.Clear();
            missionPanel.Controls.Add(mission.MissionControl);
            const string statusMessage = "Algorithm is working so hard...";
            missionStateLabel.Text = statusMessage;
            missionStateLabel.ForeColor = Color.Brown;

            currentMissionLabel.Text = mission.Title;
             
            var item = new ListViewItem(mission.Title);
            item.SubItems.Add(statusMessage);

            missionHistoryListView.Items.Insert(0, item);
        }

        private void timelapsTimer_Tick(object sender, EventArgs e)
        {
            processingTimeLabel.Text = cameraControl1.CurrentProcessingTime.ToString();
        }
    }
}
