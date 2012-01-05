using System;
using System.Runtime.InteropServices;

namespace Keyboard.PInvoke.SendInput.Native
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable InconsistentNaming
    public struct KEYBDINPUT
        // ReSharper restore InconsistentNaming
    {
        public short VirtualKey;
        public short ScanCode;
        public KeyboardFlags Flags;
        public int TimeStamp;
        public IntPtr ExtraInfo;
    }
}