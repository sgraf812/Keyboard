using System.Runtime.InteropServices;

namespace Keyboard.PInvoke.SendInput.Native
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable InconsistentNaming
    public struct HARDWAREINPUT
        // ReSharper restore InconsistentNaming
    {
        public int Message;
        public int WParam;
    }
}