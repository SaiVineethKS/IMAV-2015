using System;
using System.Windows.Threading;

namespace HighFlyers.Mission.MissionControls
{
    /// <summary>
    /// Interaction logic for LandAndStartControl.xaml
    /// </summary>
    public partial class LandAndStartControl
    {
        private Action forceStopAlgorithm;
        private readonly DispatcherTimer dispatcherTimer;
        private int secondsLeft;
        private Action takeOffFunction;

        public LandAndStartControl()
        {
            InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TickFunction();
        }

        private void TickFunction()
        {
            MainButton.Content = "TAKE OFF in " + secondsLeft + "\n" +
                                "Press button to cancel mission";
            MainButton.IsEnabled = true;
            secondsLeft--;
            if (secondsLeft < 0)
            {
                if (takeOffFunction != null)
                    takeOffFunction();
                IsFinished = true;
            }
        }

        public void Reset()
        {
            IsFinished = false;
            dispatcherTimer.Stop();
            secondsLeft = 9;
            MainButton.Content = "Waiting For Land Acknowledgement";
            MainButton.IsEnabled = false;
        }

        public bool IsFinished { get; private set; }

        private void MainButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (forceStopAlgorithm != null)
                forceStopAlgorithm();
            Reset();
        }

        public void StartTimer()
        {
            if (MainButton.IsEnabled)
                return;

            Reset();
            TickFunction();
            dispatcherTimer.Start();
        }

        public void SetForceStopAlgorithm(Action stop)
        {
            forceStopAlgorithm = stop;
        }

        public void SetTakeOff(Action takeOff)
        {
            takeOffFunction = takeOff;
        }
    }
}
