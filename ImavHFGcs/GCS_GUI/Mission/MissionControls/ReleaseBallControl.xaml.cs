using System;
using System.Globalization;
using System.Windows.Controls;

namespace HighFlyers.Mission.MissionControls
{
    /// <summary>
    /// Interaction logic for ReleaseBallControl.xaml
    /// </summary>
    public partial class ReleaseBallControl
    {
        public ReleaseBallControl()
        {
            InitializeComponent();
        }

        public event EventHandler BallReleased;

        protected virtual void OnBallReleased()
        {
            if (BallReleased != null) 
                BallReleased(this, EventArgs.Empty);
        }

        private void releaseBallButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnBallReleased();
        }

        public void SetBallCount(int ballsLeft)
        {
            ballsLeftLabel.Content = ballsLeft.ToString(CultureInfo.InvariantCulture);
        }
    }
}
