using System;
using System.Runtime.InteropServices;

namespace Keyboard.PInvoke.SendInput.Native
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable InconsistentNaming
    public struct MOUSEINPUT
        // ReSharper restore InconsistentNaming
    {
        public int DeltaX;
        public int DeltaY;
        public int MouseData;
        public MouseFlags Flags;
        public int Time;
        public IntPtr ExtraInfo;
    }
}