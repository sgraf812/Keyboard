using System.Windows.Forms;

namespace Keyboard.PInvoke
{
    public interface IKeyCodeMapper
    {
        Keys MapToVirtualKey(char c);
        char MapToChar(Keys key);
        byte MapToScanCode(Keys key);
    }
}