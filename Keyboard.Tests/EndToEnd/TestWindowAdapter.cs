using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Keyboard.PInvoke;
using Keyboard.Tests.Infrastructure;
using NUnit.Framework;

namespace Keyboard.Tests.EndToEnd
{
    class TestWindowAdapter : IDisposable
    {
        private readonly ConcurrentQueue<KeyEventArgs> _keydownEvents = new ConcurrentQueue<KeyEventArgs>();
        private readonly ConcurrentQueue<KeyPressEventArgs> _keypressEvents = new ConcurrentQueue<KeyPressEventArgs>();
        private readonly ConcurrentQueue<KeyEventArgs> _keyupEvents = new ConcurrentQueue<KeyEventArgs>();
        private readonly IKeyCodeMapper _mapper = new KeyCodeMapper();
        private readonly Poller _poller;
        private readonly AutoResetEvent _uiReady = new AutoResetEvent(false);
        private readonly Thread _uiThread;
        private readonly Form _window = new Form();

        public TestWindowAdapter(int timeoutMillis)
        {
            //ShowWindow(_window.Handle, 0);
            _uiThread = new Thread(StartUi) { IsBackground = true };
            _poller = new Poller(timeoutMillis, 1);

            _window.Shown += (s, e) => _uiReady.Set();
            _window.KeyPress += (s, e) => _keypressEvents.Enqueue(e);
            _window.KeyDown += (s, e) => _keydownEvents.Enqueue(e);
            _window.KeyUp += (s, e) => _keyupEvents.Enqueue(e);
        }

        public IntPtr WindowHandle
        {
            get { return (IntPtr) _window.Invoke((Func<IntPtr>) (() => _window.Handle)); }
        }

        public bool HasFocus
        {
            get { return (bool) _window.Invoke((Func<bool>) (() => _window.ContainsFocus)); }
        }

        [DllImport("user32")]
        private extern static bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private void StartUi()
        {
            _window.Show();
            Application.Run(_window);
        }

        public void Show()
        {
            _uiThread.Start();
            Assert.IsTrue(_uiReady.WaitOne(1000), "UI thread was not ready before timeout.");
        }

        public void Focus()
        {
            _window.Invoke((Action) (() => _window.Focus()));
        }

        public void ShouldHaveReceivedKeyPressWith(char c)
        {
            _poller.Check(new FuncProbe(() =>
            {
                KeyPressEventArgs evt;
                while (_keypressEvents.TryDequeue(out evt))
                {
                    if (evt.KeyChar == c)
                        return true;
                }
                return false;
            }) { FailureMessage = "Test window received no KeyPress message for '" + c + "'" });
        }

        public void ShouldHaveReceivedKeyUpWith(char c)
        {
            ShouldReceiveKeyEvent(c, "KeyUp", _keyupEvents);
        }

        public void ShouldHaveReceivedKeyDownWith(char c)
        {
            ShouldReceiveKeyEvent(c, "KeyDown", _keydownEvents);
        }

        private void ShouldReceiveKeyEvent(char c, string messageKind, ConcurrentQueue<KeyEventArgs> events)
        {
            Keys key = _mapper.MapToVirtualKey(c);
            var matcher = new KeyEventMatcher(key);
            _poller.Check(new FuncProbe(() =>
            {
                KeyEventArgs evt;
                while (events.TryDequeue(out evt))
                {
                    matcher.Observe(evt);
                    if (matcher.WasMatched)
                        return true;
                }
                return false;
            }) { FailureMessage = "Test window received no " + messageKind + " message for '" + c + "'" });
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            _window.Invoke((Action) (() =>
            {
                _window.Close();
                _window.Dispose();
            }));
            _uiThread.Abort();
        }

        #endregion

        #region Nested type: KeyEventMatcher

        private class KeyEventMatcher
        {
            private readonly bool _alt;
            private readonly bool _control;
            private readonly Keys _keyCode;
            private readonly bool _shift;

            public KeyEventMatcher(Keys key)
            {
                _shift = key.HasFlag(Keys.Shift);
                _control = key.HasFlag(Keys.Control);
                _alt = key.HasFlag(Keys.Alt);

                _keyCode = key & Keys.KeyCode;
            }

            public bool WasMatched { get; private set; }

            public void Observe(KeyEventArgs evt)
            {
                if (KeyStateMatchesExactly(evt))
                {
                    WasMatched = true;
                }
            }

            private bool KeyStateMatchesExactly(KeyEventArgs evt)
            {
                return evt.Control == _control
                    && evt.Shift == _shift
                        && evt.Alt == _alt
                            && evt.KeyCode == _keyCode;
            }
        }

        #endregion
    }
}