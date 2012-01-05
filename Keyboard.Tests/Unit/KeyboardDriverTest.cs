using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Keyboard.PInvoke;
using Moq;
using NUnit.Framework;

namespace Keyboard.Tests
{
    [TestFixture]
    public class KeyboardDriverTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            _keyboardMock = new Mock<IKeyboard>();
            _driver = new KeyboardDriver(_keyboardMock.Object, new KeyCodeMapper());
        }

        #endregion

        private Mock<IKeyboard> _keyboardMock;
        private KeyboardDriver _driver;

        public void TearDown()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
        }

        [Test]
        public void CanWriteDeadCharacter()
        {
            _driver.Write("€");

            _keyboardMock.Verify(x => x.KeyPress(Keys.E | Keys.Alt | Keys.Control));
        }

        [Test]
        public void CanWriteExclamationMark()
        {
            _driver.Write("!");

            _keyboardMock.Verify(x => x.KeyPress(Keys.D1 | Keys.Shift));
        }

        [Test]
        public void CanWriteSingleLowercaseLetter()
        {
            _driver.Write("a");

            _keyboardMock.Verify(x => x.KeyPress(Keys.A));
        }

        [Test]
        public void CanWriteSingleUppercaseLetter()
        {
            _driver.Write("A");

            _keyboardMock.Verify(x => x.KeyPress(Keys.A | Keys.Shift));
        }
    }
}