using System;
using System.Windows.Input;

namespace HighFlyers.Mission
{
    public abstract class Mission
    {
        protected Mission(string title)
        {
            Title = title;
        }

        public string Title { get; private set; }
        public System.Windows.Controls.Control MissionControl { get; protected set; }

        public System.IO.Stream SoundStream { private get; set; }

        public override string ToString()
        {
            return Title;
        }

        virtual public Key ShortcutKey
        {
            get
            {
                return Key.None;
            }
        }

        public virtual void CallCommand(bool b)
        {
        }

        unsafe protected abstract bool RunMethodInternal(byte* imgBuffer, int width, int height);

        unsafe public bool RunMethod(byte* imgBuffer, int width, int height)
        {
            
            bool temp = RunMethodInternal(imgBuffer, width, height);
            if (temp)
            {
                if (SoundStream != null)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                    player.Stream = SoundStream;
                    player.Stream.Seek(0, new System.IO.SeekOrigin()); // set stream position at the beginning
                    player.Play();
                }
            }
            return temp;
        }

        virtual public void Reset()
        {
        }

        virtual public void ForceStop()
        {
        }
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
