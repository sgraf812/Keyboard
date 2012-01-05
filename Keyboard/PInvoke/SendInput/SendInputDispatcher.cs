using System.Runtime.InteropServices;
using Keyboard.PInvoke.SendInput.Native;

namespace Keyboard.PInvoke.SendInput
{
    public class SendInputDispatcher : IInputDispatcher
    {
        #region Implementation of IInputDispatcher

        public unsafe void DispatchInput(params INPUT[] inputs)
        {
            PInvokeHelper.ThrowWin32ErrorIfFalse(() =>
                SendInput(inputs.Length, inputs, sizeof(INPUT)) == inputs.Length);
        }

        #endregion

        #region PInvoke

        [DllImport("user32", SetLastError = true)]
        private extern static int SendInput(int inputNumber, INPUT[] inputs, int size);

        #endregion
    }
}