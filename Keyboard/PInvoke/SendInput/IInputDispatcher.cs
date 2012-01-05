using Keyboard.PInvoke.SendInput.Native;

namespace Keyboard.PInvoke.SendInput
{
    public interface IInputDispatcher
    {
        void DispatchInput(params INPUT[] inputs);
    }
}