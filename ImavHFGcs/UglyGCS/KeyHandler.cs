using System;
using System.Collections.Generic;
using HighFlyers;

namespace UglyGCS
{
    class KeyHandler
    {
        private Dictionary<int, Action<int, bool>> keyEvents;
        private Dictionary<int, FixedCommand> keyCommands;

        public KeyHandler(Action<int, bool> sendCommand)
        {
            InitKeyEvents(sendCommand);
            InitKeyCommands();
        }

        public void DoMethod(int key, bool isKeyDown)
        {
            if (keyEvents.ContainsKey(key))
                keyEvents[key].Invoke(key, isKeyDown);
        }

        public _FrameSrame GetKeyCommand(bool isKeyDown, int key)
        {
            return isKeyDown ? keyCommands[key].Frame : keyCommands[key].BackFrame;
        }

        public float GetKeyValue(int key)
        {
            return keyCommands[key].CommandValue;
        }

        private void InitKeyCommands()
        {
            keyCommands = new Dictionary<int, FixedCommand>();

            keyCommands['w'] = new FixedCommand(_FrameSrame.CMD_START_FORWARD, _FrameSrame.CMD_STOP_FORWARD, 10.0f);
            keyCommands['r'] = new FixedCommand(_FrameSrame.CMD_START_BACKWARD, _FrameSrame.CMD_STOP_BACKWARD, 10.0f);
            keyCommands['a'] = new FixedCommand(_FrameSrame.CMD_START_LEFT, _FrameSrame.CMD_STOP_LEFT, 10.0f);
            keyCommands['s'] = new FixedCommand(_FrameSrame.CMD_START_RIGHT, _FrameSrame.CMD_STOP_RIGHT, 10.0f);
        }

        private void InitKeyEvents(Action<int, bool> sendCommand)
        {
            keyEvents = new Dictionary<int, Action<int, bool>>();
            keyEvents['w'] = sendCommand;
            keyEvents['a'] = sendCommand;
            keyEvents['r'] = sendCommand;
            keyEvents['s'] = sendCommand;
        }
    }

    struct FixedCommand
    {
        public FixedCommand(_FrameSrame frame, _FrameSrame backFrame, float value)
            : this()
        {
            Frame = frame;
            CommandValue = value;
            BackFrame = backFrame;
        }
        public _FrameSrame Frame { get; private set; }
        public _FrameSrame BackFrame { get; private set; }
        public float CommandValue { get; private set; }
    }
}
