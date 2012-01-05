using System.Collections.Generic;
using System.Windows.Forms;
using Keyboard.PInvoke.SendInput;
using Keyboard.PInvoke.SendInput.Native;
using Moq;
using NUnit.Framework;

namespace Keyboard.Tests.Unit
{
    [TestFixture]
    public class SendInputKeyboardTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _dispatcherMock = new Mock<IInputDispatcher>();
            _keyboard = new SendInputKeyboard(_dispatcherMock.Object);
            _inputs = new List<INPUT>();

            _dispatcherMock.Setup(x => x.DispatchInput(It.IsAny<INPUT[]>()))
                .Callback((INPUT[] input) => _inputs.AddRange(input));
        }

        #endregion

        private Mock<IInputDispatcher> _dispatcherMock;
        private SendInputKeyboard _keyboard;
        private List<INPUT> _inputs;

        private void TestKeyPress(Keys key)
        {
            _keyboard.KeyPress(key);

            AssertAKeyPress(key);
        }

        private void AssertAKeyPress(Keys key)
        {
            int neededInputs = 2;
            var modifiers = new Keys?[3];
            if (key.HasFlag(Keys.Shift))
            {
                modifiers[0] = Keys.ShiftKey;
                neededInputs += 2;
            }
            if (key.HasFlag(Keys.Control))
            {
                modifiers[1] = Keys.ControlKey;
                neededInputs += 2;
            }
            if (key.HasFlag(Keys.Alt))
            {
                modifiers[2] = Keys.Menu;
                neededInputs += 2;
            }

            Assert.AreEqual(neededInputs, _inputs.Count);

            int currentInput = 0;
            foreach (var modifier in modifiers)
            {
                if (modifier.HasValue)
                {
                    AssertIsKeyDownInputFor(modifier.Value, _inputs[currentInput++]);
                }
            }

            AssertIsKeyDownInputFor(key, _inputs[currentInput++]);
            AssertIsKeyUpInputFor(key, _inputs[currentInput++]);

            foreach (var modifier in modifiers)
            {
                if (modifier.HasValue)
                {
                    AssertIsKeyUpInputFor(modifier.Value, _inputs[currentInput++]);
                }
            }
        }

        private void AssertIsKeyDownInputFor(Keys key, INPUT input)
        {
            int virtualKey = (int) key & 0xff;
            Assert.AreEqual(input.Type, INPUT.InputType.Keyboard);
            Assert.AreEqual(input.Keyboard.VirtualKey, virtualKey);
            Assert.AreEqual(input.Keyboard.Flags, KeyboardFlags.None);
        }

        private void AssertIsKeyUpInputFor(Keys key, INPUT input)
        {
            Assert.AreEqual(input.Type, INPUT.InputType.Keyboard);
            Assert.AreEqual(input.Keyboard.VirtualKey, (int) key & 0xff);
            Assert.AreEqual(input.Keyboard.Flags, KeyboardFlags.KeyUp);
        }

        [Test]
        public void CanSendAModifiedKeyPress()
        {
            TestKeyPress(Keys.A | Keys.Shift | Keys.Alt);
        }

        [Test]
        public void CanSendASingleKeyDown()
        {
            _keyboard.KeyDown(Keys.A);

            AssertIsKeyDownInputFor(Keys.A, _inputs[0]);
        }

        [Test]
        public void CanSendASingleKeyPress()
        {
            TestKeyPress(Keys.A);
        }

        [Test]
        public void CanSendASingleKeyUp()
        {
            _keyboard.KeyUp(Keys.A);

            AssertIsKeyUpInputFor(Keys.A, _inputs[0]);
        }
    }
}