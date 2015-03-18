using System.Collections.Generic;

namespace UglyGCS.Mission
{
    class MissionManager
    {
        private readonly List<Mission> missions;

        public MissionManager()
        {
            missions = new List<Mission>
                {
                    new QrCodeMission("QRCodes")
                };
        }

        public List<Mission> GetMissions()
        {
            return missions;
        }
    }
}
