using System;
using System.Windows.Forms;

namespace UglyGCS.Mission
{
    public abstract class Mission
    {
        protected Mission(string title)
        {
            Title = title;
        }

        public string Title { get; private set; }
        public Control MissionControl { get; protected set; }

        public override string ToString()
        {
            return Title;
        }

        unsafe public abstract bool RunMethod(byte* imgBuffer, int width, int height);
    }

    public class MissionEventArgs : EventArgs
    {
        public Mission Mission { get; private set; }

        public MissionEventArgs(Mission mission)
        {
            Mission = mission;
        }
    }

    public delegate void MissionEventHandler(object sender, MissionEventArgs e); 
}
