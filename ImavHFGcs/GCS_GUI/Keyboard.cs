using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HighFlyers
{
    internal abstract class Keyboard
    {
        public abstract Key GetKey(Key key);    
    }

    internal class KeyboardQwerty : Keyboard
    {
        private readonly Dictionary<Key, Key> keyMap = new Dictionary<Key, Key>()
        {
            {Key.F, Key.E},
            {Key.P, Key.R},
            {Key.G, Key.T},
            {Key.J, Key.Y},
            {Key.L, Key.U},
            {Key.U, Key.I},
            {Key.Y, Key.O},
            {Key.OemSemicolon, Key.P},
            {Key.R, Key.S},
            {Key.S, Key.D},
            {Key.T, Key.F},
            {Key.D, Key.G},
            {Key.N, Key.J},
            {Key.E, Key.K},
            {Key.I, Key.L},
            {Key.O, Key.OemSemicolon},
            {Key.K, Key.N}
        }; 

        public override Key GetKey(Key key)
        {
            return keyMap.ContainsKey(key) ? keyMap[key] : key;
        }
    }

    internal class KeyboardColemak : Keyboard
    {
        public override Key GetKey(Key key)
        {
            return key;
        }
    }
}
