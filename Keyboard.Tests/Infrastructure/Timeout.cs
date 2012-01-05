using System;

namespace Keyboard.Tests.Infrastructure
{
    class Timeout
    {
        private readonly DateTime _timeoutTime;

        public Timeout(int timeoutMillis)
        {
            _timeoutTime = DateTime.Now.AddMilliseconds(timeoutMillis);
        }

        public bool HasTimedOut()
        {
            return DateTime.Now > _timeoutTime;
        }
    }
}