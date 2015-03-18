using System;
using HighFlyers.Mission.MissionControls;

namespace HighFlyers.Mission
{
    class LandAndStartMission : Mission
    {
        private readonly LandAndStartControl control;

        public LandAndStartMission(string title) : base(title)
        {
            MissionControl = new LandAndStartControl();
            control = MissionControl as LandAndStartControl;
        }

        public void SetReadyForTakeOff()
        {
            control.StartTimer();
        }

        protected override unsafe bool RunMethodInternal(byte* imgBuffer, int width, int height)
        {
            if (control.IsFinished)
            {
                control.Reset();
                return true;
            }
            return false;
        }

        public void SetForceStopAlgorithm(Action stop)
        {
            control.SetForceStopAlgorithm(stop);
        }

        public void SetTakeOffFunction(Action takeOff)
        {
            control.SetTakeOff(takeOff);
        }

        public override void ForceStop()
        {
            control.Reset();
            base.ForceStop();
        }
    }
}
