using System.Runtime.InteropServices;

namespace Keyboard.PInvoke.SendInput.Native
{
    [StructLayout(LayoutKind.Explicit)]
    // ReSharper disable InconsistentNaming
    public struct INPUT
        // ReSharper restore InconsistentNaming
    {
        [FieldOffset(0)] public InputType Type;

        [FieldOffset(sizeof(int))] public MOUSEINPUT Mouse;
        [FieldOffset(sizeof(int))] public KEYBDINPUT Keyboard;
        [FieldOffset(sizeof(int))] public HARDWAREINPUT Hardware;

        public enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }
    }
}