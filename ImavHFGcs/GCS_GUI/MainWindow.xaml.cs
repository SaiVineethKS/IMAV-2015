using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using HighFlyers.Communication;
using HighFlyers.Mission;
using HighFlyers.Various;
using DataFormat = HighFlyers.Communication.DataFormat;
using System.Diagnostics;
using ZedGraph;
using Application = System.Windows.Application;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using RichTextBox = System.Windows.Controls.RichTextBox;
using TextBox = System.Windows.Controls.TextBox;


namespace HighFlyers
{
    /// <summary>
    /// Interaction logic for MainWindow.public
    /// </summary>
    partial class MainWindow
    {
        private readonly Thread sendThread;
        private readonly ManualResetEvent dataPresentEvent = new ManualResetEvent(false);
        private Queue<byte[]> queue;
        private RS232 port;
        readonly ProtoBuffer readBuffer = new ProtoBuffer();
        readonly ObservableCollection<Proto> framesCollection = new ObservableCollection<Proto>();
        readonly ObservableCollection<MissionHistory> missionHistory = new ObservableCollection<MissionHistory>();
        private readonly DispatcherTimer processingFrameTimer;
        private readonly KeyHandler keyHandler;
        private readonly Dictionary<int, string> errorMessages = new Dictionary<int, string> {{255, "Status not received"}};
        private List<Point> driftPoints = new List<Point>();
        private Point tempPoint;
        private bool rs232DebugView;
        private const string Flytronic = "flytronic";
        private const string HighFlyers = "highflyers";
        private string currentMagicString = String.Empty;
        private readonly GraphDataFrame[] graphsFrames = new GraphDataFrame[3];
        private readonly ZedGraphControl[] graphsControls;
        private readonly ComboBox[] plotComboBoxes;
        private readonly int[] graphIterators = new int[3];
        private readonly string[] rpiStatusMap = { "OK", "Bad first frame" };
        private readonly Control[] disableKeyCommandsControls;
        private StreamWriter logFileWriter = null;
        private readonly DispatcherTimer countdownTimerRx;
        private readonly DispatcherTimer countdownTimerTx;
        private const int Interval = 300;
        private bool shiftPressed = false;
        private readonly HashSet<Key> pressedKeys = new HashSet<Key>();
        private readonly List<ToDoElement> beforeFlightList = new List<ToDoElement>();
        private int rpiCurrentStatus = -1;
        private int trimsUpdated = 0;

        public ObservableCollection<Proto> FramesCollection
        { get { return framesCollection; } }

        public ObservableCollection<MissionHistory> MissionHistoryCollection
        { get { return missionHistory;} }

        public class MissionHistory
        {
            public string MissionTitle { get; set; }
            public string MissionResult { get; set; }
        }

       
        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            keyHandler = new KeyHandler(SendCommand, "QWERTY");
            InitKeyHandler();

            var missionManager = new MissionManager(SendCommand);
            var m = missionManager.GetMission("Precision Landing") as LandAndStartMission;
            if (m != null)
            {
                m.SetForceStopAlgorithm(StartStopMission);
                m.SetTakeOffFunction(TakeOff);
            }

            graphsControls = new[] {graphControl1, graphControl2, graphControl3};
            plotComboBoxes = new[]{plot1ComboBox, plot2ComboBox, plot3ComboBox};
            commandComboBox.ItemsSource = Enum.GetNames(typeof(_FrameSrame));
            commandComboBox.SelectedIndex = 0;

            foreach (var cb in plotComboBoxes)
            {
                cb.ItemsSource = Enum.GetNames(typeof(_FrameSrame));  
            }

            readBuffer.FrameReady += readBuffer_FrameReady;

            foreach (string s in RS232.ListOfAvailablePorts)
                ListofPortsComboBox.Items.Add(s);

            for (int i = 0; i < 4; i++)
                cameraListComboBox.Items.Add(i.ToString(CultureInfo.InvariantCulture));

            keycombobox.Items.Add("QWERTY");
            keycombobox.Items.Add("Colemak");

            // LOAD USER SETTINGS:
            LoadSetting();

            camera1.AlgorithmFinished += (sender, args) =>
            {
                keyHandler.RemoveCommand(args.Mission.ShortcutKey);
                ChangeMissionState(args.Mission.Title, "Mission finished!", Colors.Green, "Start");
                processingFrameTimer.Stop();
            };

            processingFrameTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)};
            processingFrameTimer.Tick += processingFrameTimer_Tick;

            missionComboBox.ItemsSource = missionManager.GetMissions();

            FLOATRadioButton.IsChecked = true;

            PreviewKeyDown += MainWindow_PreviewKeyDown;
            PreviewKeyUp += MainWindow_PreviewKeyUp;

            sendThread = new Thread(SendInBackground)
                {
                    IsBackground = true,
                    Name = "SendingThread",
                    Priority = ThreadPriority.Normal
                };

            readBuffer_FrameReady_Status(new Proto {Command = _FrameSrame.CMD_SYSTEM_STATE, ValueInt32 = 255});

            Pid.UpdatePidFields += UpdatePid;
            Pid.SendCommand += SendCommand;

            tempPoint = new Point(0,0);

            disableKeyCommandsControls = new Control[] { FilterCommand, commandValueTextBox };

            countdownTimerRx = new DispatcherTimer(){Interval = new TimeSpan(0,0,0,0,Interval)};
            countdownTimerRx.Tick +=countdownTimerRX_Tick;
            countdownTimerTx = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0,Interval) };
            countdownTimerTx.Tick +=countdownTimerTX_Tick;

            beforeFlightList.Add(new ToDoElement("Is port opened?", () => port != null && port.IsOpen,
                                                 () => OpenPortButton_Click(null, null)));
            beforeFlightList.Add(new ToDoElement("Is camera started?", () => camera1.IsStarted,
                                                 () => cameraButton_Click(null, null)));
            beforeFlightList.Add(new ToDoElement("Is Raspberry enabled?", () => rpiCurrentStatus == 0,
                                                 () => RpiStatusIndicator_MouseUp(null, null)));
            beforeFlightList.Add(new ToDoElement("Is engine turned on?", EngineTurnedOn));
            beforeFlightList.Add(new ToDoElement("Are trims updated?", () => trimsUpdated == 2,
                                                 () => button4_Click(null, null)));
            beforeFlightList.Add(new ToDoElement("Is quadro on the ground?", () => altitudeIndicator1.Value < 0.01));

        }

        private bool CheckBeforeFlightList()
        {
            var todoWindow = new TodoWindow(beforeFlightList);
            todoWindow.ShowDialog();
            return todoWindow.DialogResult == true;
        }

        private void InitKeyHandler()
        {
            keyHandler.AppendCommand(Key.F1, (key, isDown, turbo) => InvokeIndexedMission(0, !isDown));
            keyHandler.AppendCommand(Key.F2, (key, isDown, turbo) => InvokeIndexedMission(1, !isDown));
            keyHandler.AppendCommand(Key.F3, (key, isDown, turbo) => InvokeIndexedMission(2, !isDown));
            keyHandler.AppendCommand(Key.F5, (key, isDown, turbo) => SwitchTab(0, !isDown));
            keyHandler.AppendCommand(Key.F6, (key, isDown, turbo) => SwitchTab(1, !isDown));
            keyHandler.AppendCommand(Key.F7, (key, isDown, turbo) => SwitchTab(2, !isDown));
            keyHandler.AppendCommand(Key.F8, (key, isDown, turbo) => SwitchTab(3, !isDown));
            keyHandler.AppendCommand(Key.Tab, (key, isDown, turbo) => SwitchTab((tabControl1.SelectedIndex+1)%4, !isDown));
            keyHandler.AppendCommand(Key.C, (key, isDown, turbo) =>
                {
                    if (!isDown)
                        return;
                    camera1.IsCrosshair = !camera1.IsCrosshair;
                });
        }

        private void SwitchTab(int tabIndex, bool invoke)
        {
            if (!invoke) return;
            tabControl1.SelectedIndex = tabIndex;
        }

        private void InvokeIndexedMission(int index, bool invoke)
        {
            if (invoke || index >= missionComboBox.Items.Count) return;
            missionComboBox.SelectedIndex = index;
            StartStopMission();
        }

        private void countdownTimerTX_Tick(object sender, EventArgs e)
        {
            TxIndicator.Fill = new SolidColorBrush(Colors.Silver);
            countdownTimerTx.Stop();
        }

        private void countdownTimerRX_Tick(object sender, EventArgs e)
        {
            RxIndicator.Fill = new SolidColorBrush(Colors.Silver);
            countdownTimerRx.Stop();
        }

        private void RxIndicatorOn()
        {
            if (countdownTimerRx.IsEnabled)
                countdownTimerRx.Stop();

            countdownTimerRx.Interval = new TimeSpan(0,0,0,0,Interval);

            if (RxIndicator.Dispatcher.CheckAccess())
                RxIndicator.Fill = new SolidColorBrush(Colors.Lime);
            else
            {
                RxIndicator.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => {RxIndicator.Fill = new SolidColorBrush(Colors.Lime); }));
            }

            countdownTimerRx.Start();
        }

        private void TxIndicatorOn()
        {
            if (countdownTimerTx.IsEnabled)
                countdownTimerTx.Stop();

            countdownTimerTx.Interval = new TimeSpan(0,0,0,0,Interval);

            if (TxIndicator.Dispatcher.CheckAccess())
                TxIndicator.Fill = new SolidColorBrush(Colors.Lime);
            else
            {
                TxIndicator.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => { TxIndicator.Fill = new SolidColorBrush(Colors.Lime); }));
            }
            countdownTimerTx.Start();
        }

        private void LoadSetting()
        {
            var rsport = Properties.Settings.Default.RS232Port;
            ListofPortsComboBox.SelectedIndex = ListofPortsComboBox.Items.Contains(rsport) ? ListofPortsComboBox.Items.IndexOf(rsport) : 0;

            var cam = Properties.Settings.Default.Camera;
            cameraListComboBox.SelectedIndex = cameraListComboBox.Items.Contains(cam) ? cameraListComboBox.Items.IndexOf(cam) : 0;

            var framecb = Properties.Settings.Default.FrameCombo0;
            plot1ComboBox.SelectedIndex = plot1ComboBox.Items.Contains(framecb) ? plot1ComboBox.Items.IndexOf(framecb) : 0;

            framecb = Properties.Settings.Default.FrameCombo1;
            plot2ComboBox.SelectedIndex = plot2ComboBox.Items.Contains(framecb) ? plot2ComboBox.Items.IndexOf(framecb) : 0;

            framecb = Properties.Settings.Default.FrameCombo2;
            plot3ComboBox.SelectedIndex = plot3ComboBox.Items.Contains(framecb) ? plot3ComboBox.Items.IndexOf(framecb) : 0;

            var state = Properties.Settings.Default.ShowDrift;
            showLinesCheckBox.IsChecked = state;

            state = Properties.Settings.Default.DebugView;
            debugViewCheckBox.IsChecked = state;

            var kb = Properties.Settings.Default.keyboard;
            if (keycombobox.Items.Contains(kb))
            {
                keycombobox.SelectedIndex = keycombobox.Items.IndexOf(kb);
                keyHandler.SetKeyboard(kb);
                InitKeyHandler();
            }
            else
            {
                keycombobox.SelectedIndex = 0;
            }
        }

        void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (disableKeyCommandsControls.Any(t => t.IsFocused))
            {
                return;
            }

            pressedKeys.Remove(e.Key);

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                shiftPressed = false;
                foreach (var k in pressedKeys)
                {
                    keyHandler.DoMethod(k, true, false);
                }
            }

            keyHandler.DoMethod(e.Key, false, shiftPressed);
        }

        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (disableKeyCommandsControls.Any(t => t.IsFocused))
            {
                return;
            }

            if (e.IsRepeat) return;

            if (keyHandler.IsValidKey(e.Key) && keyHandler.GetKeyInfo(e.Key).TurboEnabled)
                pressedKeys.Add(e.Key);

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                foreach (var k in pressedKeys)
                {
                    keyHandler.DoMethod(k, true, true);
                }
                shiftPressed = true;
            }

            var c = char.ToLower(e.Key.ToString()[0]);

            if (currentMagicString.Length < Flytronic.Length && c == Flytronic[currentMagicString.Length])
                currentMagicString += c;
            else if (currentMagicString.Length < HighFlyers.Length && c == HighFlyers[currentMagicString.Length])
                currentMagicString += c;
            else
                currentMagicString = String.Empty;

            switch (currentMagicString)
            {
                case Flytronic:
                    SwitchTheme(currentMagicString);
                    currentMagicString = string.Empty;
                    break;
                case HighFlyers:
                    SwitchTheme(HighFlyers);
                    currentMagicString = string.Empty;
                    break;
            }

            keyHandler.DoMethod(e.Key, true, shiftPressed);
        }

        private void SendCommand(Key key, bool isKeyDown, bool turbo)
        {
            SendAndAndDisplay(keyHandler.GetKeyCommand(isKeyDown, key),
                              keyHandler.GetKeyValue(key, isKeyDown, turbo));
        }

        private void SendCommand(_FrameSrame proto, float value)
        {
            SendAndAndDisplay(proto, value);
        }
        
        #region RS232
  
        private void SendInBackground()
        {
            while (IsPortOpened())
            {
                dataPresentEvent.WaitOne();
                while (queue.Count > 0 && IsPortOpened())
                {
                    byte[] temp = queue.Dequeue();
                    port.SendData(temp);
                    TxIndicatorOn();
                }
                if (dataPresentEvent != null)
                    dataPresentEvent.Reset();
                Thread.Yield();
            }
        }

        // list of available ports
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            ListofPortsComboBox.Items.Clear();

            foreach (string s in RS232.ListOfAvailablePorts)
                ListofPortsComboBox.Items.Add(s);

            ListofPortsComboBox.SelectedIndex = 0;
        }

        private bool IsPortOpened()
        {
            return port != null && port.IsOpen;
        }
        
        // Open or close port
        private void OpenPortButton_Click(object sender, RoutedEventArgs e)
        {          
            try
            {
                if (!IsPortOpened())
                {
                    port = new RS232(Convert.ToString(ListofPortsComboBox.SelectedItem), "57600", "8");
                    port.OpenPort();
                    port.DataReceived += port_DataReceived;
                    ConnectPortButton.Content = "Disconnect";
                    IsConnectedIndicator.Fill = new SolidColorBrush(Colors.Lime);
                    queue = new Queue<byte[]>();
                    if ((sendThread.ThreadState & System.Threading.ThreadState.Unstarted) == System.Threading.ThreadState.Unstarted) 
                        sendThread.Start();
                }
                else
                {
                    port.StopReceiving = true;                    
                    queue.Clear();
                    port.ClosePort();
                    dataPresentEvent.Reset();
                    ConnectPortButton.Content = "Connect";
                    IsConnectedIndicator.Fill = new SolidColorBrush(Colors.Red);
                }
            }
            catch (Exception exc)
            {
                LogMessage("Problem with open/close port: " + exc.Message);
            }
        }


        private void port_DataReceived(byte[] rcvd)
        {
            try
            {
                foreach (byte b in rcvd)
                {
                    readBuffer.Append(b);
                }

                UpdateRichTextBox(rcvd, ReceiveRichTextBox);
                RxIndicatorOn();
            }
            catch (Exception exc)
            {
                LogMessage("Problem in data received: " + exc.Message);
            }
        }

        private string GetHexView(byte[] bytes)
        {
            string displayStr = StringParser.ByteToDisplay(bytes, DataFormat.Hex).Replace("{x", "");
            return displayStr.Replace('}', ' ');
        }

        private void UpdateRichTextBox(byte[] r, RichTextBox richTextBox)
        {
            if (!rs232DebugView)
                return;
            if (richTextBox.Dispatcher.CheckAccess())
            {
                if (r == null)
                    return;
                var tr = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                
                if(tr.Text.Length > 1000)
                    richTextBox.Document.Blocks.Clear();

                richTextBox.AppendText(GetHexView(r));
                richTextBox.ScrollToEnd();
            }
            else
            {
                richTextBox.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => UpdateRichTextBox(r, richTextBox)));
            }
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

        private byte[] SendAndAndDisplay(_FrameSrame command, int value)
        {
            byte[] bytes = SendCommandToPort(command, value);
            UpdateRichTextBox(bytes, SendRichTextBox);

            return bytes;
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

        private byte[] SendAndAndDisplay(_FrameSrame command, float value)
        {
            byte[] bytes = SendCommandToPort(command, value);
            UpdateRichTextBox(bytes, SendRichTextBox);

            return bytes;
        }

        private byte[] SendCommandToPort(Proto proto)
        {
            if (!IsPortOpened())
                return null;

            ProtoBuffer buffer = proto.ToBuffer();
            byte[] sent = buffer.PrepareToSend();
            queue.Enqueue(sent);
            dataPresentEvent.Set();
            return sent;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsPortOpened())
                {
                    byte[] toSend = (FLOATRadioButton.IsChecked == true)
                                  ? SendCommandToPort(
                                      (_FrameSrame)
                                      Enum.Parse(typeof(_FrameSrame), commandComboBox.SelectedItem.ToString()),
                                      float.Parse(commandValueTextBox.Text.Replace('.', ',')))
                                  : SendCommandToPort(
                                      (_FrameSrame)
                                      Enum.Parse(typeof(_FrameSrame), commandComboBox.SelectedItem.ToString()),
                                      int.Parse(commandValueTextBox.Text));

                    UpdateRichTextBox(toSend, SendRichTextBox);
                }
            }
            catch (Exception exc)
            {
                LogMessage("Problem in sending data: " + exc.Message);
            }
        }

        #endregion

        
        #region Camera

        private void cameraButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!camera1.IsStarted)
                {
                    camera1.Start(cameraListComboBox.SelectedIndex);
                    cameraButton.Content = @"Close camera";
                }
                else
                {
                    camera1.Stop();
                    cameraButton.Content = @"Open camera";
                }
            }
            catch (Exception ex)
            {
                LogMessage("Problem in open/close camera: " + ex.Message);
            }
        }

        private void switchCameraButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchCamera();
        }

        private void SwitchCamera()
        {
            SendAndAndDisplay(_FrameSrame.CMD_SWITCH_CAMERA, 0);
        }

        #endregion


        #region Mission
        private void startMissionButton_Click(object sender, RoutedEventArgs e)
        {
            StartStopMission();
        }

        void StartStopMission()
        {
            if (camera1.CurrentMission != null)
            {
                camera1.CurrentMission.ForceStop();
                ChangeMissionState(camera1.CurrentMission.Title, "Algorithm was stopped", Colors.Red, "Start");
                keyHandler.RemoveCommand(camera1.CurrentMission.ShortcutKey);
                camera1.SetMission(null);
                currentMissionLabel.Content = "none";
                currentMissionNameLabel.Content = "none";
                return;
            }

            var mission = missionComboBox.SelectedItem as Mission.Mission;
            if (mission == null) return;

            camera1.SetMission(mission);
            if (mission.ShortcutKey != Key.None)
                SetShortcutMission(mission);
            processingFrameTimer.Start();

            missionStackPanel.Children.RemoveRange(0, missionStackPanel.Children.Count);
            missionStackPanel.Children.Add(mission.MissionControl);
            ChangeMissionState(mission.Title, "Algorithm is working so hard...", Colors.Yellow, "Stop");
            currentMissionLabel.Content = camera1.CurrentMission.Title;
            currentMissionNameLabel.Content = camera1.CurrentMission.Title;
        }

        private void ChangeMissionState(string missionName, string statusMessage, Color statusColor, string buttonTitle)
        {
            missionStateLabel.Content = statusMessage;
            missionStateLabel.Foreground = new SolidColorBrush(statusColor);
            missionHistory.Add(new MissionHistory { MissionTitle = missionName, MissionResult = statusMessage });
            startMissionButton.Content = buttonTitle;
        }

        private void SetShortcutMission(Mission.Mission mission)
        {
            keyHandler.AppendCommand(mission.ShortcutKey, (key, b, turbo) => mission.CallCommand(b));
        }


        private void imgDebugWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var customWindow = new Window {SizeToContent = SizeToContent.WidthAndHeight};
            var pb = new PictureBox();
            var grid = new Grid();
            var winhost = new System.Windows.Forms.Integration.WindowsFormsHost {Width = 640, Height = 480, Child = pb};
            grid.Children.Add(winhost);
            camera1.SetPictureBox(pb);
            customWindow.Content = grid;
            customWindow.Show();
        }

        private void processingFrameTimer_Tick(object sender, EventArgs e)
        {
            currentProcessingTimeLabel.Content = camera1.CurrentProcessingTime.ToString();
        }

        #endregion


        # region PID

        private void PIDCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PIDCombo.SelectedValue != null)
                Pid.PidInfoDemand(PIDCombo.SelectedValue.ToString());
        }

        void SafeSet(TextBox tb, string value)
        {
            if (tb.Dispatcher.CheckAccess())
            {
                tb.Text = value;
            }
            else
            {
                tb.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => SafeSet(tb, value)));
            }
        }

        bool SafeCheck(ComboBox cb, string what)
        {
            if (cb.Dispatcher.CheckAccess())
            {
                return PIDCombo.SelectedValue != null && PIDCombo.SelectedValue.ToString() == what;
            }
            return (bool)cb.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Func<bool>(() => SafeCheck(cb, what)));
        }

        private void UpdatePid(string[] data, string what)
        {
            if(!SafeCheck(PIDCombo, what))
                return;

            if (data == null)
            {
                KpText.Text = KiText.Text = KpText.Text = "";
                return;
            }
            Debug.Assert(data.Length == 3);
            // invoke?
            // yup
            // PM: wpf is still magic for me
            SafeSet(KpText, data[0]);
            SafeSet(KiText, data[1]);
            SafeSet(KdText, data[2]);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string kp = KpText.Text;
            string ki = KiText.Text;
            string kd = KdText.Text;

            try
            {
                Pid.SendAndStore(PIDCombo.SelectedValue.ToString(), kp, ki, kd);
            }
            catch (Exception ex)
            {
                LogMessage("Invalid value in pid sending: " + ex.Message);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you REALLY want to save parameters to flash? Don't do it during flight.", "Saving Parameters To Flash", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SendCommand(_FrameSrame.CMD_SAVE_TO_FLASH, 42f);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            SendCommand(_FrameSrame.CMD_GET_PID_SETTINGS, 42);
            pidReceiveStatusLabel.Content = "Waiting for response...";
            Pid.ResetStorage();
            if (PIDCombo.SelectedValue != null)
                Pid.PidInfoDemand(PIDCombo.SelectedValue.ToString());
            //    FramesCollection.CollectionChanged += FramesCollection_CollectionChanged;
        }

        void FramesCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var frames = e.NewItems;
            foreach (Proto frame in frames)
            {
                Pid.Update(frame.Command, frame.ValueFloat.ToString(CultureInfo.InvariantCulture));
            }
            PIDCombo.SelectedIndex = -1;
            PIDCombo.InvalidateVisual();
        }

        # endregion


        # region Drift

        private void showLinesCheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            if (showLinesCheckBox.IsChecked == true)
            {
                camera1.DrawPoints(driftPoints);
                Properties.Settings.Default.ShowDrift = true;
            }
            else
            {
                camera1.ClearPoints();
                Properties.Settings.Default.ShowDrift = false;
            }

            Properties.Settings.Default.Save();
        }

        private void startStopDrift_Click(object sender, RoutedEventArgs e)
        {
            SendAndAndDisplay(_FrameSrame.CMD_RESET_DRIFT, 0);
            ClearDriftPoints();
        }

        private void clearDriftTraceButton_Click(object sender, RoutedEventArgs e)
        {
            ClearDriftPoints();
        }

        private void ClearDriftPoints()
        {
            camera1.ClearPoints();
            driftPoints = new List<Point>();
        }
        # endregion

        private void takeoffButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBeforeFlightList())
                TakeOff();
        }

        private void TakeOff()
        {
            SendAndAndDisplay(_FrameSrame.CMD_QUADRO_START, 0);
        }

        private bool EngineTurnedOn()
        {
            return engineLabel.Content.ToString() == "STOP";
        }

        private void engineButton_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show("Do you REALLY want to change engine's state.",
                                "Saving Parameters To Flash", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            
            if (!EngineTurnedOn())
            {
                var sent = SendAndAndDisplay(_FrameSrame.CMD_ENGINE_ON, 0);
                if (sent != null)
                    engineLabel.Content = "STOP";
            }
            else
            {
                var sent = SendAndAndDisplay(_FrameSrame.CMD_ENGINE_OFF, 0);
                if (sent != null)
                    engineLabel.Content = "START";
            }
        }

        private void LogMessage(string msg)
        {
            if (logRichTextBox.Dispatcher.CheckAccess())
            {
                logRichTextBox.AppendText(msg + "\n");
                logRichTextBox.ScrollToEnd();
            }
            else
            {
                logRichTextBox.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => LogMessage(msg)));
            }
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsPortOpened())
                port.ClosePort();

            if (camera1.IsStarted)
                camera1.Stop();
            if (sendThread.ThreadState == System.Threading.ThreadState.Stopped)
                dataPresentEvent.Set();
            if (sendThread.ThreadState == System.Threading.ThreadState.Running)
            {
                sendThread.Join();
                Thread.Yield();
            }
            dataPresentEvent.Dispose();
        }

        private void resetSystemStateButton_Click(object sender, RoutedEventArgs e)
        {
            statusTextBox.Foreground = new SolidColorBrush(Colors.Lime);
            statusTextBox.Text = "System OK";
        }

        private void debugViewCheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            rs232DebugView = debugViewCheckBox.IsChecked.HasValue && debugViewCheckBox.IsChecked.Value;
            
        }

        private void clearDebugViewButton_Click(object sender, RoutedEventArgs e)
        {
            FramesCollection.Clear();
            ReceiveRichTextBox.Document.Blocks.Clear();
            SendRichTextBox.Document.Blocks.Clear();
        }

        private void SwitchTheme(string s)
        {
            if (s == Flytronic)
            {
                Application.Current.Resources.Source = new Uri("/CustomExpression1.xaml", UriKind.RelativeOrAbsolute);
                mainGrid.Background = new SolidColorBrush(Color.FromRgb(213,0,139));
                statusBar.Background = new SolidColorBrush(Color.FromRgb(80, 5, 54));

            }
            else
            {
                Application.Current.Resources.Source = new Uri("/ExpressionDark.xaml", UriKind.RelativeOrAbsolute);
                mainGrid.Background = new SolidColorBrush(Color.FromRgb(68, 68, 68));
                statusBar.Background = new SolidColorBrush(Color.FromRgb(34,34,34));
            }
        }

        private void cameraListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.Camera = cameraListComboBox.SelectedValue.ToString();
            Properties.Settings.Default.Save();
        }

        private void ListofPortsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListofPortsComboBox.SelectedValue == null)
                return;
            Properties.Settings.Default.RS232Port = ListofPortsComboBox.SelectedValue.ToString();
            Properties.Settings.Default.Save();
        }

        private void plot1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo == null) return;

            if (combo.SelectedValue == null)
                return;

            int index = Array.IndexOf(plotComboBoxes, combo);
            graphsFrames[index] = new GraphDataFrame((_FrameSrame)Enum.Parse(typeof(_FrameSrame), combo.SelectedValue.ToString()));
            graphIterators[index] = 0;

            if (index == 0)
                Properties.Settings.Default.FrameCombo0 = combo.SelectedValue.ToString();
            else if (index == 1)
                Properties.Settings.Default.FrameCombo1 = combo.SelectedValue.ToString();
            else
                Properties.Settings.Default.FrameCombo2 = combo.SelectedValue.ToString();
                
            Properties.Settings.Default.Save();

        }

        private void ClearFilterCommand_Click(object sender, RoutedEventArgs e)
        {
            FilterCommand.Text = "";
        }

        private void RpiStatusIndicator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RpiStatusIndicator.Fill = new SolidColorBrush(Colors.Silver);
            SendAndAndDisplay(_FrameSrame.CMD_RPI_HELLO_STATUS, 0);
        }

        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Document",
                    DefaultExt = ".text",
                    Filter = "Text documents (.csv)|*.csv"
                };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                logToFileNameTextBox.Text = dlg.FileName;
            }
        }

        private void logStartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (logFileWriter == null)
                {
                    logFileWriter = new StreamWriter(logToFileNameTextBox.Text);
                    LogMessage("Loggin start");
                    logStartButton.Content = "Stop";
                    logToFileNameTextBox.IsEnabled = false;
                    selectFileButton.IsEnabled = false;
                }
                else
                {
                    logFileWriter.Close();
                    LogMessage("Finish loggin");
                    logFileWriter = null;
                    logToFileNameTextBox.IsEnabled = true;
                    selectFileButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage("Error during loggin: " + ex.Message);
            }
            
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            SendAndAndDisplay(_FrameSrame.CMD_SET_TRIM_FROM_APKA, 0);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            SendAndAndDisplay(_FrameSrame.CMD_GET_TRIMS, 0);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            SendAndAndDisplay(_FrameSrame.CMD_QUADRO_LANDING, 0);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            keyHandler.SetKeyboard(keycombobox.SelectedValue.ToString());
            InitKeyHandler();
            Properties.Settings.Default.keyboard = keycombobox.SelectedValue.ToString();
            Properties.Settings.Default.Save();
        }

        private void PidToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.WriteAllText("pid.dat", Pid.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show("Couldn't write to file! " + Environment.NewLine + ex.Message);
            }
        }

        #region trims
        private const float TrimUpDownValue = 0.05f;
        private void forwardTrimButton_Click(object sender, RoutedEventArgs e)
        {
            SendAndAndDisplay(_FrameSrame.CMD_PITCH_TRIM, (float)numericUpDownTrimPitch.Value - TrimUpDownValue);
        }

        private void backwardTrimButton_Click(object sender, RoutedEventArgs e)
        {
            SendAndAndDisplay(_FrameSrame.CMD_PITCH_TRIM, (float)numericUpDownTrimPitch.Value + TrimUpDownValue);
        }

        private void leftTrimButton_Click(object sender, RoutedEventArgs e)
        {
            SendAndAndDisplay(_FrameSrame.CMD_ROLL_TRIM, (float)numericUpDownTrimRoll.Value - TrimUpDownValue);
        }

        private void rightTrimButton_Click(object sender, RoutedEventArgs e)
        {
            SendAndAndDisplay(_FrameSrame.CMD_ROLL_TRIM, (float)numericUpDownTrimRoll.Value + TrimUpDownValue);
        }
        #endregion

        private void PidFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string vals = File.ReadAllText("pid.dat");
                Pid.FromString(vals);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't read from file! " + Environment.NewLine + ex.Message);
            }
        }
    }

}
