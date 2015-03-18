using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HighFlyers.Communication;
using System.IO;
using System.Diagnostics;

namespace HighFlyers.Various
{
    public static class Pid
    {
        public static Action<string[], string> UpdatePidFields;
        public static Action<_FrameSrame, float> SendCommand;

        private static Dictionary<string, string[]> storage = new Dictionary<string,string[]>();

        private static void LocalUpdate(string what, int which, string data)
        {
            if(!storage.ContainsKey(what))
                storage.Add(what, new string[3]);
            string[] t = storage[what];
            t[which] = data;
            storage[what] = t;

            PidInfoDemand(what);
        }

        public static void ResetStorage()
        {
            storage = new Dictionary<string, string[]>();
        }

        public static bool Update(_FrameSrame what, string data)
        {//this is wrong, but its so late...
            bool wasChanged = false;
            switch (what)
            {
                case _FrameSrame.CMD_PID_ACRO_ROLL_KP:
                    LocalUpdate("AcroRoll", 0, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_ACRO_ROLL_KI:
                    LocalUpdate("AcroRoll", 1, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_ACRO_ROLL_KD:
                    LocalUpdate("AcroRoll", 2, data);
                    wasChanged = true;
                    break;

                case _FrameSrame.CMD_PID_ACRO_PITCH_KP:
                    LocalUpdate("AcroPitch", 0, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_ACRO_PITCH_KI:
                    LocalUpdate("AcroPitch", 1, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_ACRO_PITCH_KD:
                    LocalUpdate("AcroPitch", 2, data);
                    wasChanged = true;
                    break;

                case _FrameSrame.CMD_PID_ACRO_YAW_KP:
                    LocalUpdate("AcroYaw", 0, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_ACRO_YAW_KI:
                    LocalUpdate("AcroYaw", 1, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_ACRO_YAW_KD:
                    LocalUpdate("AcroYaw", 2, data);
                    wasChanged = true;
                    break;

                case _FrameSrame.CMD_PID_STABLE_PITCH_KP:
                    LocalUpdate("StablePitch", 0, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_STABLE_PITCH_KI:
                    LocalUpdate("StablePitch", 1, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_STABLE_PITCH_KD:
                    LocalUpdate("StablePitch", 2, data);
                    wasChanged = true;
                    break;

                case _FrameSrame.CMD_PID_STABLE_ROLL_KP:
                    LocalUpdate("StableRoll", 0, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_STABLE_ROLL_KI:
                    LocalUpdate("StableRoll", 1, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_STABLE_ROLL_KD:
                    LocalUpdate("StableRoll", 2, data);
                    wasChanged = true;
                    break;

                case _FrameSrame.CMD_PID_ALT_KP:
                    LocalUpdate("Alt", 0, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_ALT_KI:
                    LocalUpdate("Alt", 1, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_ALT_KD:
                    LocalUpdate("Alt", 2, data);
                    wasChanged = true;
                    break;

                case _FrameSrame.CMD_PID_VEL_KP:
                    LocalUpdate("Vel", 0, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_VEL_KI:
                    LocalUpdate("Vel", 1, data);
                    wasChanged = true;
                    break;
                case _FrameSrame.CMD_PID_VEL_KD:
                    LocalUpdate("Vel", 2, data);
                    wasChanged = true;
                    break;
            }
            return wasChanged;
        }

        public static void PidInfoDemand(string what)
        {
            string[] data = null;
            if (what != null)
            {
                if (storage.ContainsKey(what))
                    data = storage[what];
                //in cpp it would create empty entry, i prefer not to risk
            }
            if (UpdatePidFields != null)
                UpdatePidFields(data, what);
        }

        public static void SendAndStore(string what, string Kp, string Ki, string Kd)
        {
            string[] oldVals = storage[what];
            storage[what] = new string[] { Kp, Ki, Kd };
            bool updateKp = true;
            bool updateKi = true;
            bool updateKd = true;
            if (oldVals != null)
            {
                if (Kp == oldVals[0])
                    updateKp = false;
                if (Ki == oldVals[0])
                    updateKi = false;
                if (Kd == oldVals[0])
                    updateKd = false;
            }
            Proto frame = new Proto();
            List<Proto> frames = new List<Proto>();
            StringBuilder enumBuilder = new StringBuilder();
            enumBuilder.Append("CMD_PID_");

            if (what != null)
            {
                if (what != "Vel" && what != "Alt")
                {
                    int i;
                    for (i = 1; i < what.Length; i++)
                    {
                        if (char.IsUpper(what[i]))
                            break;
                    }
                    string beg = what.Remove(i);
                    string end = what.Remove(0, beg.Length);

                    enumBuilder.Append(beg.ToUpper() + "_");
                    enumBuilder.Append(end.ToUpper() + "_");
                }
                else
                {
                    enumBuilder.Append(what.ToUpper());
                    enumBuilder.Append("_");
                }
                _FrameSrame _kp = (_FrameSrame) Enum.Parse(typeof (_FrameSrame), enumBuilder.ToString() + "KP");
                _FrameSrame _ki = (_FrameSrame) Enum.Parse(typeof (_FrameSrame), enumBuilder.ToString() + "KI");
                _FrameSrame _kd = (_FrameSrame) Enum.Parse(typeof (_FrameSrame), enumBuilder.ToString() + "KD");

                Proto PFrame = new Proto();
                PFrame.Command = _kp;
                Proto IFrame = new Proto();
                IFrame.Command = _ki;
                Proto DFrame = new Proto();
                DFrame.Command = _kd;

                if (updateKp)
                {
                    PFrame.ValueFloat = float.Parse(Kp, CultureInfo.InvariantCulture);
                    frames.Add(PFrame);
                }
                if (updateKi)
                {
                    IFrame.ValueFloat = float.Parse(Ki, CultureInfo.InvariantCulture);
                    frames.Add(IFrame);
                }
                if (updateKd)
                {
                    DFrame.ValueFloat = float.Parse(Kd, CultureInfo.InvariantCulture);
                    frames.Add(DFrame);
                }
                if (SendCommand != null)
                {
                    foreach (Proto f in frames)
                    {
                        SendCommand(f.Command, f.ValueFloat);
                    }
                }
            }
        }
        private static string[] names = { "AcroRoll", "AcroPitch", "AcroYaw", "StableRoll", "StablePitch", "Alt", "Vel" };
        public new static string ToString()
        {
            //Enum.GetNames(typeof(_FrameSrame)).Where(x => x.Contains("PID") && !x.Contains("GET")).ToArray();
            Array.Sort(names);
            StringWriter writer = new StringWriter();
            
            foreach (var name in names)
            {
                string[] data = storage[name];
                foreach (var num in data)
                {
                    writer.WriteLine(num);
                }
            }
            return writer.ToString();
        }

        public static void FromString(string input)
        {
            string[] data = input.Split('\n');
            Debug.Assert(data.Length == 24);
            int i = 0;
            foreach (var name in names)
            {
                string[] vals = {data[i++], data[i++], data[i++]};
                SendAndStore(name, vals[0], vals[1], vals[2]);
                UpdatePidFields(vals, name);
            }
        }
    }
}
