using HighFlyers.Mission.MissionControls;

namespace HighFlyers.Mission
{
    class DummyMission : Mission
    {
        private readonly Algorithms.DummyMission dummyMission = new Algorithms.DummyMission();
        public DummyMission(string title) : base(title)
        {
            MissionControl = new DummyControl();
        }

        protected override unsafe bool RunMethodInternal(byte* imgBuffer, int width, int height)
        {
            dummyMission.DoMission(imgBuffer, width, height);

            return false;
        }
    }
}
