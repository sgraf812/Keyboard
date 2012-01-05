using System.Windows.Forms;
using Keyboard.PInvoke;

namespace Keyboard
{
    public class KeyboardDriver : IKeyboardDriver
    {
        private readonly IKeyboard _keyboard;
        private readonly IKeyCodeMapper _mapper;

        public KeyboardDriver(IKeyboard keyboard, IKeyCodeMapper mapper)
        {
            _mapper = mapper;
            _keyboard = keyboard;
        }

        #region Implementation of IKeyboardDriver

        public IKeyboard Keyboard
        {
            get { return _keyboard; }
        }

        public void Write(string word)
        {
            foreach (char c in word)
            {
                Keys key = _mapper.MapToVirtualKey(c);
                _keyboard.KeyPress(key);
            }
        }

        #endregion
    }
}