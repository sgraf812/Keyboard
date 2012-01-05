using System;
using System.Runtime.InteropServices;

namespace Keyboard.PInvoke.Messaging
{
    public class WindowsMessageEmitter : IMessageEmitter
    {
        #region Implementation of IMessageEmitter

        public void PostMessage(IntPtr windowHandle, WindowMessages message, IntPtr wParam, UIntPtr lParam)
        {
            PInvokeHelper.ThrowWin32ErrorIfFalse(() =>
                NativePostMessage(windowHandle, message, wParam, lParam));
        }

        public void SendMessage(IntPtr windowHandle, WindowMessages message, IntPtr wParam, UIntPtr lParam)
        {
            NativeSendMessage(windowHandle, message, wParam, lParam);
        }

        #endregion

        #region PInvoke

        [DllImport("user32", EntryPoint = "PostMessage", SetLastError = true)]
        private extern static bool NativePostMessage(IntPtr hWnd, WindowMessages msg, IntPtr wParam, UIntPtr lParam);

        [DllImport("user32", EntryPoint = "SendMessage", SetLastError = true)]
        private extern static int NativeSendMessage(IntPtr hWnd, WindowMessages msg, IntPtr wParam, UIntPtr lParam);

        #endregion
    }
}