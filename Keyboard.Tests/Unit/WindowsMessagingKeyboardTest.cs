using System;
using System.Windows.Forms;
using Keyboard.PInvoke;
using Keyboard.PInvoke.Messaging;
using Moq;
using NUnit.Framework;

namespace Keyboard.Tests.Unit
{
    [TestFixture]
    public class WindowsMessagingKeyboardTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _emitterMock = new Mock<IMessageEmitter>();
            _mapperMock = new Mock<IKeyCodeMapper>();
            _keyboard = new MessagingKeyboard(WindowHandle, _emitterMock.Object, _mapperMock.Object);
        }

        #endregion

        private static readonly IntPtr WindowHandle = new IntPtr(123);
        private Mock<IMessageEmitter> _emitterMock;
        private Mock<IKeyCodeMapper> _mapperMock;
        private MessagingKeyboard _keyboard;

        private void ShouldReceiveNullMessage()
        {
            _emitterMock.Verify(x =>
                x.SendMessage(WindowHandle, WindowMessages.WM_NULL, It.IsAny<IntPtr>(),
                    It.IsAny<UIntPtr>()));
        }

        private void ShouldReceiveKeyDown(Keys key)
        {
            ShouldReceiveKeyMessage(WindowMessages.WM_KEYDOWN, key);
        }

        private void ShouldReceiveKeyUp(Keys key)
        {
            ShouldReceiveKeyMessage(WindowMessages.WM_KEYUP, key);
        }

        private void ShouldReceiveKeyMessage(WindowMessages message, Keys key)
        {
            _emitterMock.Verify(x =>
                x.PostMessage(WindowHandle, message, new IntPtr((int) key), It.IsAny<UIntPtr>()));
        }

        private void ShouldReceiveKeyDown(Keys key, bool alt = false, bool repeat = false, ushort repeatCount = 1)
        {
            ShouldReceiveKeyMessage(WindowMessages.WM_KEYDOWN, key, alt, repeat, false, repeatCount);
        }

        private void ShouldReceiveKeyUp(Keys key, bool alt = false, bool repeat = false, ushort repeatCount = 1)
        {
            ShouldReceiveKeyMessage(WindowMessages.WM_KEYUP, key, alt, repeat, true, repeatCount);
        }

        private void ShouldReceiveKeyMessage(WindowMessages message, Keys key, bool alt, bool repeat, bool up,
            ushort repeatCount)
        {
            uint expectedLParam = repeatCount;
            if (alt) expectedLParam |= 0x20000000;
            if (repeat) expectedLParam |= 0x40000000;
            if (up) expectedLParam |= 0x80000000;

            _emitterMock.Verify(x =>
                x.PostMessage(WindowHandle, message, new IntPtr((int) key), ValueWithoutScanCode(expectedLParam)));
        }

        private void ShouldReceiveKeyDownWithScancode(Keys key, byte scancode)
        {
            ShouldReceiveKeyMessage(WindowMessages.WM_KEYDOWN, key, scancode);
        }

        private void ShouldReceiveKeyUpWithScancode(Keys key, byte scancode)
        {
            ShouldReceiveKeyMessage(WindowMessages.WM_KEYUP, key, scancode);
        }

        private void ShouldReceiveKeyMessage(WindowMessages message, Keys key, byte scancode)
        {
            _emitterMock.Verify(x =>
                x.PostMessage(WindowHandle, message, new IntPtr((int) key), ItHasScanCode(scancode)));
        }

        private static UIntPtr ItHasScanCode(byte value)
        {
            return Match.Create<UIntPtr>(p => ((((uint) p) & 0x00ff0000) >> 16) == value);
        }

        private static UIntPtr ValueWithoutScanCode(uint value)
        {
            return Match.Create<UIntPtr>(p => (((uint) p) & 0xff00ffff) == value);
        }

        [Test]
        public void CanSendASingleKeyDown()
        {
            _keyboard.KeyDown(Keys.A);

            ShouldReceiveKeyDown(Keys.A, repeatCount: 1);
        }

        [Test]
        public void CanSendASingleKeyStroke()
        {
            bool keyDown = false;
            bool keyUp = false;
            _emitterMock.Setup(x =>
                x.PostMessage(WindowHandle, WindowMessages.WM_KEYDOWN, new IntPtr((int) Keys.A), ValueWithoutScanCode(1)))
                .Callback(() => keyDown = true);
            _emitterMock.Setup(x =>
                x.PostMessage(WindowHandle, WindowMessages.WM_KEYUP, new IntPtr((int) Keys.A),
                    ValueWithoutScanCode(0xC0000001)))
                .Callback(() =>
                {
                    Assert.IsTrue(keyDown, "key was released before it was pressed");
                    keyUp = true;
                });

            _keyboard.KeyPress(Keys.A);

            Assert.IsTrue(keyUp, "no full key press happened");
        }

        [Test]
        public void CanSendASingleKeyUp()
        {
            _keyboard.KeyUp(Keys.A);

            ShouldReceiveKeyUp(Keys.A, repeatCount: 1);
        }

        [Test]
        public void CanSendRepeatedKeyDownMessages()
        {
            _keyboard.KeyDown(Keys.A);
            _keyboard.KeyDown(Keys.A);

            ShouldReceiveKeyDown(Keys.A, repeat: false);
            ShouldReceiveKeyDown(Keys.A, repeat: true);
        }

        [Test]
        public void SendsScanCodeWithEachMessage()
        {
            _mapperMock.Setup(x => x.MapToScanCode(Keys.A)).Returns(0x1E);

            _keyboard.KeyDown(Keys.A);

            ShouldReceiveKeyDownWithScancode(Keys.A, 0x1E);
        }

        [Test]
        public void SetsAltFlagIfAltIsPressed()
        {
            _keyboard.KeyDown(Keys.Menu);
            _keyboard.KeyDown(Keys.A);
            _keyboard.KeyUp(Keys.A);

            ShouldReceiveKeyDown(Keys.A, alt: true);
            ShouldReceiveKeyUp(Keys.A, alt: true, repeat: true);
        }

        [Test]
        public void SplitsVirtualKeyPressIntoModifierAndActualKeyStrokes()
        {
            _keyboard.KeyPress(Keys.Oem102 | Keys.Shift); // like < + shift = > on a german keyboard

            ShouldReceiveKeyDown(Keys.ShiftKey);
            ShouldReceiveKeyDown(Keys.Oem102);
            ShouldReceiveKeyUp(Keys.Oem102);
            ShouldReceiveKeyUp(Keys.ShiftKey);
        }
    }
}