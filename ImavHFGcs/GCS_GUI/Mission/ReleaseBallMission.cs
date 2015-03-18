using System;
using System.Windows.Input;
using HighFlyers.Mission.MissionControls;

namespace HighFlyers.Mission
{
    sealed class ReleaseBallMission : Mission
    {
        private const int MaxBallsCount = 6;
        private int ballsLeft = MaxBallsCount;
        private readonly Action<_FrameSrame, float> sendFrameAction;
        private readonly ReleaseBallControl control;

        public ReleaseBallMission(string title, Action<_FrameSrame, float> sendFrameAction)
            : base(title)
        {
            this.sendFrameAction = sendFrameAction;
            MissionControl = new ReleaseBallControl();
            control = MissionControl as ReleaseBallControl;
            control.BallReleased += BallReleased;
            control.SetBallCount(ballsLeft);
        }

        private void BallReleased(object sender, EventArgs e)
        {
            CallCommand(false);
        }

        protected override unsafe bool RunMethodInternal(byte* imgBuffer, int width, int height)
        {
            if (ballsLeft == 0)
            {
                ballsLeft = MaxBallsCount;
                control.SetBallCount(ballsLeft);
                return true;
            }

            return false;
        }

        public override Key ShortcutKey
        {
            get
            {
                return Key.H;
            }
        }

        public override void CallCommand(bool b)
        {
            if (b)
                return;

            sendFrameAction(_FrameSrame.CMD_RELEASE_BALL, 10.0f);
            ballsLeft--;
            if (ballsLeft < 0) ballsLeft = 0;
            control.SetBallCount(ballsLeft);
        }

        public override void Reset()
        {
            ballsLeft = MaxBallsCount;
            control.SetBallCount(ballsLeft);
        }
    }
}
