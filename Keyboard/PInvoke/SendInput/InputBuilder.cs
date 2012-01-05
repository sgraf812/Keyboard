using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Keyboard.PInvoke.SendInput.Native;

namespace Keyboard.PInvoke.SendInput
{
    public class InputBuilder
    {
        private readonly List<INPUT> _inputs = new List<INPUT>();

        public INPUT[] BuildArray()
        {
            return _inputs.ToArray();
        }

        public InputBuilder AddKeyDown(Keys key)
        {
            var down = new INPUT
            {
                Type = INPUT.InputType.Keyboard,
                Keyboard = new KEYBDINPUT
                {
                    VirtualKey = (short) ((int) key & 0xff),
                    ScanCode = 0,
                    Flags = KeyboardFlags.None,
                    TimeStamp = 0,
                    ExtraInfo = IntPtr.Zero
                }
            };

            _inputs.Add(down);
            return this;
        }

        public InputBuilder AddKeyUp(Keys key)
        {
            var up = new INPUT
            {
                Type = INPUT.InputType.Keyboard,
                Keyboard = new KEYBDINPUT
                {
                    VirtualKey = (short) ((int) key & 0xff),
                    ScanCode = 0,
                    Flags = KeyboardFlags.KeyUp,
                    TimeStamp = 0,
                    ExtraInfo = IntPtr.Zero
                }
            };

            _inputs.Add(up);
            return this;
        }
    }
}