using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Keyboard.PInvoke
{
    public class KeyCodeMapper : IKeyCodeMapper
    {
        #region Implementation of IKeyCodeMapper

        public Keys MapToVirtualKey(char c)
        {
            short vkey = VkKeyScan(c);
            var ret = (Keys) (vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) ret |= Keys.Shift;
            if ((modifiers & 2) != 0) ret |= Keys.Control;
            if ((modifiers & 4) != 0) ret |= Keys.Alt;
            return ret;
        }

        public char MapToChar(Keys virtualKey)
        {
            virtualKey = virtualKey & Keys.KeyCode;
            return (char) MapVirtualKey(virtualKey, MAPVK_VK_TO_CHAR);
        }

        public byte MapToScanCode(Keys virtualKey)
        {
            virtualKey = virtualKey & Keys.KeyCode;
            return (byte) MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC);
        }

        #endregion

        #region PInvoke

        // ReSharper disable InconsistentNaming
        private const uint MAPVK_VK_TO_VSC = 0;
        private const uint MAPVK_VK_TO_CHAR = 2;
        // ReSharper restore InconsistentNaming

        [DllImport("user32")]
        private extern static short VkKeyScan(char c);

        [DllImport("user32")]
        private extern static int MapVirtualKey(Keys key, uint mapType);

        #endregion
    }
}