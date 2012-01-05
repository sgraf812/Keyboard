using Keyboard.PInvoke;
using Keyboard.PInvoke.Messaging;
using Keyboard.PInvoke.SendInput;
using NUnit.Framework;

namespace Keyboard.Tests.EndToEnd
{
    [TestFixture]
    public class KeyboardDriverEndToEndTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _window = new TestWindowAdapter(TimeoutMillis);
            _window.Show();
            _driver = new KeyboardDriver(WindowsMessagingKeyboard(), new KeyCodeMapper());
        }

        [TearDown]
        public void DisposeWindowAdapter()
        {
            _window.Dispose();
        }

        #endregion

        private const int TimeoutMillis = 100;
        private KeyboardDriver _driver;
        private TestWindowAdapter _window;

        private IKeyboard WindowsMessagingKeyboard()
        {
            return new MessagingKeyboard(_window.WindowHandle, new WindowsMessageEmitter(), new KeyCodeMapper());
        }

        private IKeyboard SendInputKeyboard()
        {
            return new SendInputKeyboard(new SendInputDispatcher());
        }

        private void WindowShouldHaveReceivedKeyPressSequence(char c)
        {
            _window.ShouldHaveReceivedKeyDownWith(c);
            _window.ShouldHaveReceivedKeyPressWith(c);
            _window.ShouldHaveReceivedKeyUpWith(c);
        }

        [Test]
        public void SendsInputToTargetWindowWithoutChangingFocus()
        {
            var focusWindow = new TestWindowAdapter(TimeoutMillis);

            focusWindow.Show();
            focusWindow.Focus();
            Assert.IsFalse(_window.HasFocus, "The target window has focus.");

            _driver.Write("a");

            WindowShouldHaveReceivedKeyPressSequence('a');
            Assert.IsFalse(_window.HasFocus, "The target window gained focus.");
        }

        [Test]
        public void TargetWindowReceivesCharacterSequence()
        {
            _driver.Write("1234");

            WindowShouldHaveReceivedKeyPressSequence('1');
            WindowShouldHaveReceivedKeyPressSequence('2');
            WindowShouldHaveReceivedKeyPressSequence('3');
            WindowShouldHaveReceivedKeyPressSequence('4');
        }

        [Test]
        public void TargetWindowReceivesLowercaseInput()
        {
            _driver.Write("a");

            WindowShouldHaveReceivedKeyPressSequence('a');
        }

        [Test]
        public void TargetWindowReceivesModifiedCharInput()
        {
            _driver.Write("@");

            WindowShouldHaveReceivedKeyPressSequence('@');
        }

        [Test]
        public void TargetWindowReceivesUppercaseInput()
        {
            _driver.Write("A");

            WindowShouldHaveReceivedKeyPressSequence('A');
        }
    }
}