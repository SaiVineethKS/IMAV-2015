using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using HighFlyers.Communication;
using HighFlyers.Mission;
using HighFlyers.Various;
using ZedGraph;

namespace HighFlyers
{
    partial class MainWindow
    {
        void readBuffer_FrameReady(object sender, ProtoEventArgs e)
        {
            CheckAccessAndInvoke(e.Proto, CommandsListView, readBuffer_FrameReady_ListView);

            switch (e.Proto.Command)
            {
                case _FrameSrame.CMD_BATTERY_VOLTAGE:
                    CheckAccessAndInvoke(e.Proto, batteryVoltage1, readBuffer_FrameReady_BatteryVoltage);
                    break;
                case _FrameSrame.CMD_SET_ALTITUDE:
                    CheckAccessAndInvoke(e.Proto, altitudeIndicator1, readBuffer_FrameReady_AltitudeSet);
                    break;
                case _FrameSrame.CMD_SYSTEM_STATE:
                    CheckAccessAndInvoke(e.Proto, statusTextBox, readBuffer_FrameReady_Status);
                    break;
                case _FrameSrame.CMD_REF_POINT_X:
                    CheckAccessAndInvoke(e.Proto, camera1, readBuffer_FrameReady_XPoint);
                    //CheckAccessAndInvoke(e.Proto, camera1, readBuffer_FrameReady_Raspberry);
                    break;
                case _FrameSrame.CMD_REF_POINT_Y:
                    CheckAccessAndInvoke(e.Proto, camera1, readBuffer_FrameReady_YPoint);
                    break;
                case _FrameSrame.CMD_ENGINE_ON:
                    CheckAccessAndInvoke(e.Proto, engineButton, readBuffer_FrameReady_EngineON);
                    break;
                case _FrameSrame.CMD_ENGINE_OFF:
                    CheckAccessAndInvoke(e.Proto, engineLabel, readBuffer_FrameReady_Engine);
                    CheckAccessAndInvoke(e.Proto, engineButton, readBuffer_FrameReady_EngineON);
                    break;
                case _FrameSrame.CMD_RPI_HELLO_STATUS:
                    CheckAccessAndInvoke(e.Proto, RpiStatusLabel, readBuffer_FrameReady_RpiStatus);
                    break;
                case _FrameSrame.CMD_ROLL_TRIM:
                    LoadTrimValue(numericUpDownTrimRoll, e.Proto);
                    break;
                case _FrameSrame.CMD_PITCH_TRIM:
                    LoadTrimValue(numericUpDownTrimPitch, e.Proto);
                    break;
                case _FrameSrame.CMD_RESET_DRIFT:
                    ClearDriftPoints();
                    break;
            }

			switch (e.Proto.Command)
			{
				case _FrameSrame.CMD_PLOT_ADD_0:
                case _FrameSrame.CMD_PLOT_ADD_1:
                case _FrameSrame.CMD_PLOT_ADD_2:
                case _FrameSrame.CMD_PLOT_ADD_3:
				case _FrameSrame.CMD_VERTICAL_ALTITUDE:
				case _FrameSrame.CMD_VERTICAL_SPEED:
                case _FrameSrame.CMD_SET_ALTITUDE:
                    LogToFile(e.Proto);
                    break;
			}
			
            bool updatedPid = Pid.Update(e.Proto.Command, e.Proto.ValueFloat.ToString(CultureInfo.InvariantCulture));

            if (updatedPid)
                CheckAccessAndInvoke(e.Proto, pidReceiveStatusLabel, proto =>
                                                                     pidReceiveStatusLabel.Content = "Status OK");

            foreach (var graph in graphsControls.Where(graph => graph != null))
            {
                ZedGraphControl cpGraph = graph;
                CheckAccessAndInvoke(e.Proto, graph, p => UpdateGraph(p, cpGraph));
            }
        }

        private void readBuffer_FrameReady_EngineON(Proto proto)
        {
            engineButton.Background = new SolidColorBrush(proto.Command == _FrameSrame.CMD_ENGINE_ON ? Colors.Green : Color.FromRgb(0x59, 0x59, 0x59));
        }

        void CheckAccessAndInvoke(Proto e, Control control, Action<Proto> doit)
        {
            if (control.Dispatcher.CheckAccess())
            {
                doit(e);
            }
            else
            {
                control.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => doit(e)));
            }
        }

        void CheckAccessAndInvoke(Proto e, System.Windows.Forms.Control control, Action<Proto> doit)
        {
            if (!control.InvokeRequired)
            {
                doit(e);
            }
            else
            {
                control.Invoke(
                    new Action(() => doit(e)));
            }
        }

        private void LoadTrimValue(UserControls.NumericUpDown p0, Proto proto)
        {
            CheckAccessAndInvoke(proto, p0, p => p0.Value = proto.ValueFloat);

            if(trimsUpdated < 2)
                trimsUpdated++;
        }

        private void LogToFile(Proto proto)
        {
            if (logFileWriter != null)
            {
                logFileWriter.Write(proto.Command.ToString() + "," + proto.ValueFloat.ToString(CultureInfo.InvariantCulture) + ",");
                if (proto.Command == _FrameSrame.CMD_PLOT_ADD_3)
                    logFileWriter.WriteLine();
            }
        }

        private void readBuffer_FrameReady_RpiStatus(Proto proto)
        {
            RpiStatusLabel.Content = proto.ValueInt32 >= rpiStatusMap.Length
                ? "Not recognized"
                : rpiStatusMap[proto.ValueInt32];
            rpiCurrentStatus = proto.ValueInt32;
            RpiStatusLabel.Content += " (" + proto.ValueInt32.ToString(CultureInfo.InvariantCulture) + ")";
            RpiStatusIndicator.Fill =
                new SolidColorBrush(proto.ValueInt32 != 0 ? Colors.Red : Colors.Lime);
        }

        private void readBuffer_FrameReady_Raspberry(Proto proto)
        {
            var x = (short)(proto.ValueInt32 >> 16);
            var y = (short)(proto.ValueInt32 & 0xFFFF);

            var protoX = new Proto { Command = _FrameSrame.CMD_REF_POINT_X, ValueFloat = x };
            var protoY = new Proto { Command = _FrameSrame.CMD_REF_POINT_Y, ValueFloat = y };

            readBuffer_FrameReady_XPoint(protoX);
            readBuffer_FrameReady_YPoint(protoY);
        }

        private void readBuffer_FrameReady_Engine(Proto proto)
        {
            switch (proto.Command)
            {
                case _FrameSrame.CMD_ENGINE_OFF:
                    engineLabel.Content = "START";
                    CheckAccessAndInvoke(proto, takeoffButton, proto1 => takeoffButton.Content = "TAKE OFF");
                    break;
                case _FrameSrame.CMD_ENGINE_ON:
                    engineLabel.Content = "STOP";
                    break;
            }
        }

        private void UpdateGraph(Proto proto, ZedGraphControl graph)
        {
            int j = Array.IndexOf(graphsControls, graph);
            GraphDataFrame t = graphsFrames[j];
            if (t == null || t.Frame != proto.Command)
                return;

            GraphPane pane = graph.GraphPane;
            if (pane == null)
                return;

            pane.CurveList.Clear();
            t.AddPoint(proto.ValueFloat);

            Scale xScale = pane.XAxis.Scale;
            int iterator = (graphIterators[j] > 80) ? graphIterators[j] : 80;
            xScale.Min = iterator - 80;
            xScale.Max = 20 + iterator;

            var list = new PointPairList();
            for (int i = Math.Max(0, t.Iterator - 80); i < t.Iterator; i++)
            {
                list.Add(i, t.GetList()[i]);
            }

            pane.AddCurve(t.Frame.ToString(), list, System.Drawing.Color.Navy, SymbolType.None);
            graph.AxisChange();
            graph.Refresh();
            graphIterators[j]++;
        }
        void readBuffer_FrameReady_ListView(Proto proto)
        {
            if (!rs232DebugView)
                return;
            if (FramesCollection.Count > 100)
                FramesCollection.Clear();
            else if (CommandsListView.Items.Count > 0)
                CommandsListView.ScrollIntoView(CommandsListView.Items[CommandsListView.Items.Count - 1]);

            string[] filter = FilterCommand.Text.Split(' ');

            if (filter.Length > 0)
            {
                if (filter.Any(t => proto.Command.ToString().Contains(t.ToUpper())))
                {
                    FramesCollection.Add(proto);
                }
            }
            else
                FramesCollection.Add(proto);
        }

        void readBuffer_FrameReady_BatteryVoltage(Proto proto)
        {
            batteryVoltage1.Value = proto.ValueFloat;
        }

        void readBuffer_FrameReady_Altitude(Proto proto)
        {
            altitudeIndicator1.Value = proto.ValueFloat;
        }

        void readBuffer_FrameReady_AltitudeSet(Proto proto)
        {
            altitudeIndicator1.DValue = proto.ValueFloat;
        }

        void readBuffer_FrameReady_Status(Proto proto)
        {
            if (proto.ValueInt32 != 0)
            {
                string errorMessage = "ERROR: " +
                                     (errorMessages.ContainsKey(proto.ValueInt32)
                                          ? errorMessages[proto.ValueInt32]
                                          : "Unknow error code " +
                                            proto.ValueInt32.ToString(CultureInfo.InvariantCulture));

                LogMessage(statusTextBox.Text);
                statusTextBox.Foreground = new SolidColorBrush(Colors.Red);
                statusTextBox.Text = errorMessage;
            }
            else
            {
                statusTextBox.Foreground = new SolidColorBrush(Colors.Lime);
                statusTextBox.Text = "System OK";
            }
        }

        void readBuffer_FrameReady_XPoint(Proto proto)
        {
            tempPoint.X = proto.ValueFloat;
        }

        void readBuffer_FrameReady_YPoint(Proto proto)
        {
            tempPoint.Y = proto.ValueFloat;
            // should I check if x was sent???

            driftPoints.Add(tempPoint);
            tempPoint = new Point(0, 0);

            if (showLinesCheckBox.IsChecked == true)
                camera1.DrawPoints(driftPoints);
        }

    }
}
