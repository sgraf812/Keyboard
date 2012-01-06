using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Keyboard.PInvoke.Messaging
{
    public class MessagingKeyboard : IKeyboard
    {
        private readonly IMessageEmitter _emitter;
        private readonly IKeyCodeMapper _mapper;
        private readonly ISet<Keys> _pressedKeys = new HashSet<Keys>();
        private readonly IntPtr _windowHandle;

        public MessagingKeyboard(IntPtr windowHandle, IMessageEmitter emitter, IKeyCodeMapper mapper)
        {
            _windowHandle = windowHandle;
            _emitter = emitter;
            _mapper = mapper;
        }

        #region Implementation of IKeyboard

        public void KeyUp(Keys key)
        {
            uint lParam = MakeLParamFor(key, 1, true);
            PostKeyMessage(key & Keys.KeyCode, WindowMessages.WM_KEYUP, lParam);
            _pressedKeys.Remove(key);
        }

        public void KeyDown(Keys key)
        {
            uint lParam = MakeLParamFor(key, 1, false);
            PostKeyMessage(key & Keys.KeyCode, WindowMessages.WM_KEYDOWN, lParam);
            //Thread.Sleep(15);
            _pressedKeys.Add(key);
        }

        public void KeyPress(Keys key)
        {
            var modifiers = new ModifierExtractor(key);

            foreach (Keys modifier in modifiers.AsEnumerable())
                KeyDown(modifier);

            ActualKeyPress(key);

            foreach (Keys modifier in modifiers.AsEnumerable())
                KeyUp(modifier);
        }

        private uint MakeLParamFor(Keys key, uint repeatCount, bool keyUp)
        {
            uint result = repeatCount;

            byte scancode = _mapper.MapToScanCode(key);
            result |= (uint) (scancode << 16);

            if (_pressedKeys.Contains(Keys.Menu))
                result |= 0x20000000;
            if (_pressedKeys.Contains(key))
                result |= 0x40000000;
            if (keyUp)
                result |= 0x80000000;

            return result;
        }

        private void ActualKeyPress(Keys key)
        {
            KeyDown(key);
            KeyUp(key);
        }

        private void PostKeyMessage(Keys key, WindowMessages message, uint lParam)
        {
            _emitter.PostMessage(_windowHandle, message,
                new IntPtr((int) key), new UIntPtr(lParam));
        }

        private void SendNullMessage()
        {
            _emitter.SendMessage(_windowHandle, WindowMessages.WM_NULL, IntPtr.Zero, UIntPtr.Zero);
        }

        #endregion
    }
}