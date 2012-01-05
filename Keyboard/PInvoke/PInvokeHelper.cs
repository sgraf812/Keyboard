using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Keyboard.PInvoke
{
    static class PInvokeHelper
    {
        public static void ThrowWin32ErrorIfFalse(Func<bool> frame)
        {
            if (!frame())
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}