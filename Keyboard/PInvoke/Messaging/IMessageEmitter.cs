using System;

namespace Keyboard.PInvoke.Messaging
{
    public interface IMessageEmitter
    {
        void PostMessage(IntPtr windowHandle, WindowMessages message, IntPtr wParam, UIntPtr lParam);
        void SendMessage(IntPtr windowHandle, WindowMessages message, IntPtr wParam, UIntPtr lParam);
    }
}