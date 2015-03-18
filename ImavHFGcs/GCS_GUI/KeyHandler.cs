using System;
using System.Collections.Generic;
using System.Windows.Input;


namespace HighFlyers
{
    class KeyHandler
    {
        private Dictionary<Key, Action<Key, bool, bool>> keyEvents;
        private Dictionary<Key, FixedCommand> keyCommands;
        private Keyboard keyboard;
        private readonly Action<Key, bool, bool> sendCommand;

        public KeyHandler(Action<Key, bool, bool> sendCommand, string keyb)
        {
            this.sendCommand = sendCommand;
            SetKeyboard(keyb);
        }

        public void SetKeyboard(string keyb)
        {
            switch (keyb)
            {
                case "QWERTY":
                    keyboard = new KeyboardQwerty();
                    break;
                case "Colemak":
                    keyboard = new KeyboardColemak();
                    break;
            }
            InitKeyEvents(sendCommand);
            InitKeyCommands();
        }

        public void AppendCommand(Key key, Action<Key, bool, bool> command)
        {
            keyEvents[keyboard.GetKey(key)] = command;
        }

        public void RemoveCommand(Key key)
        {
            keyEvents.Remove(key);
        }

        public void DoMethod(Key key, bool isKeyDown, bool turbo)
        {
            if (keyEvents.ContainsKey(key))
                keyEvents[key].Invoke(key, isKeyDown, turbo);
        }

        public _FrameSrame GetKeyCommand(bool isKeyDown, Key key)
        {
            return isKeyDown ? keyCommands[key].Frame : keyCommands[key].BackFrame;
        }

        public bool IsValidKey(Key key)
        {
            return keyCommands.ContainsKey(key);
        }

        public float GetKeyValue(Key key, bool startCommand, bool turbo)
        {
            return (startCommand ? keyCommands[key].CommandStartValue : keyCommands[key].CommandStopValue) * (keyCommands[key].TurboEnabled && turbo ? 3 : 1);
        }

        private void InitKeyCommands()
        {
            keyCommands = new Dictionary<Key, FixedCommand>();

            keyCommands[keyboard.GetKey(Key.W)] = new FixedCommand(_FrameSrame.CMD_START_FORWARD, -1.5f, _FrameSrame.CMD_STOP_FORWARD, 0, true);
            keyCommands[keyboard.GetKey(Key.R)] = new FixedCommand(_FrameSrame.CMD_START_BACKWARD, 1.5f, _FrameSrame.CMD_STOP_BACKWARD, 0, true);
            keyCommands[keyboard.GetKey(Key.A)] = new FixedCommand(_FrameSrame.CMD_START_LEFT, -1.5f, _FrameSrame.CMD_STOP_LEFT, 0, true);
            keyCommands[keyboard.GetKey(Key.S)] = new FixedCommand(_FrameSrame.CMD_START_RIGHT, 1.5f, _FrameSrame.CMD_STOP_RIGHT, 0, true);
            keyCommands[keyboard.GetKey(Key.Q)] = new FixedCommand(_FrameSrame.CMD_START_ROTATE_LEFT, 20.0f, _FrameSrame.CMD_STOP_ROTATE_LEFT, 0);
            keyCommands[keyboard.GetKey(Key.F)] = new FixedCommand(_FrameSrame.CMD_START_ROTATE_RIGHT, -20.0f, _FrameSrame.CMD_STOP_ROTATE_RIGHT, 0);
            keyCommands[keyboard.GetKey(Key.Z)] = new FixedCommand(_FrameSrame.CMD_START_UP, 10.0f, _FrameSrame.CMD_STOP_UP, 0, true);
            keyCommands[keyboard.GetKey(Key.X)] = new FixedCommand(_FrameSrame.CMD_START_DOWN, 10.0f, _FrameSrame.CMD_STOP_DOWN, 0, true);
            keyCommands[keyboard.GetKey(Key.N)] = new FixedCommand(_FrameSrame.CMD_RESET_DRIFT, 0.0f, _FrameSrame.CMD_LINE_A, 0, false);
            keyCommands[keyboard.GetKey(Key.E)] = new FixedCommand(_FrameSrame.CMD_SWITCH_CAMERA, 0.0f, _FrameSrame.CMD_LINE_A, 0, false);
        }

        private void InitKeyEvents(Action<Key, bool, bool> sendCommand)
        {
            keyEvents = new Dictionary<Key, Action<Key, bool, bool>>();
            keyEvents[keyboard.GetKey(Key.W)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.A)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.R)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.S)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.Q)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.F)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.Z)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.X)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.N)] = sendCommand;
            keyEvents[keyboard.GetKey(Key.E)] = sendCommand;
        }

        public FixedCommand GetKeyInfo(Key key)
        {
            return keyCommands[key];
        }
    }

    struct FixedCommand
    {
        public FixedCommand(_FrameSrame frame, float startValue, _FrameSrame backFrame, float stopValue, bool turboEnabled = false)
            : this()
        {
            Frame = frame;
            CommandStartValue = startValue;
            CommandStopValue = stopValue;
            BackFrame = backFrame;
            TurboEnabled = turboEnabled;
        }
        public _FrameSrame Frame { get; private set; }
        public _FrameSrame BackFrame { get; private set; }
        public float CommandStartValue { get; private set; }
        public float CommandStopValue { get; private set; }
        public bool TurboEnabled { get; private set; }
    }
}
