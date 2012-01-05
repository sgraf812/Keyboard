using System.Threading;
using NUnit.Framework;

namespace Keyboard.Tests.Infrastructure
{
    public class Poller
    {
        private readonly int _pollDelayMillis;
        private readonly int _timeoutMillis;

        public Poller(int timeoutMillis, int pollDelayMillis)
        {
            _timeoutMillis = timeoutMillis;
            _pollDelayMillis = pollDelayMillis;
        }

        public void Check(IProbe probe)
        {
            var timeout = new Timeout(_timeoutMillis);

            while (!probe.IsSatisfied)
            {
                if (timeout.HasTimedOut())
                {
                    Assert.Fail(DescribeFailureOf(probe));
                }
                Thread.Sleep(_pollDelayMillis);

                probe.Sample();
            }
        }

        private string DescribeFailureOf(IProbe probe)
        {
            return "Timeout of " + _timeoutMillis + "ms expired.\n"
                + probe.FailureMessage;
        }
    }
}