using System;
using System.Collections.Generic;

namespace HighFlyers.Mission
{
    class MissionManager
    {
        private readonly List<Mission> missions;

        public MissionManager(Action<_FrameSrame, float> sendFrameAction)
        {
            missions = new List<Mission>
                {
                    new QrCodeMission("QRCodes"),
                    new ReleaseBallMission("Release Balls", sendFrameAction){SoundStream = Properties.Resources.release},
                    new LandAndStartMission("Precision Landing"),
                    new DummyMission("Dummy mission")
                };
        }

        public List<Mission> GetMissions()
        {
            return missions;
        }

        public Mission GetMission(string missionTitle)
        {
            return missions.Find(mission => mission.Title == missionTitle);
        }
    }
}
