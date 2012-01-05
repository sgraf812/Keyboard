using System.Windows.Forms;

namespace Keyboard
{
    public interface IKeyboard
    {
        void KeyDown(Keys key);
        void KeyUp(Keys key);
        void KeyPress(Keys key);
    }
}