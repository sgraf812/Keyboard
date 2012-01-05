using System;

namespace Keyboard.Tests.Infrastructure
{
    public class FuncProbe : IProbe
    {
        private readonly Func<bool> _predicate;

        public FuncProbe(Func<bool> predicate)
        {
            _predicate = predicate;
            IsSatisfied = false;
            FailureMessage = "";
        }

        #region Implementation of IProbe

        public bool IsSatisfied { get; private set; }

        public string FailureMessage { get; set; }

        public void Sample()
        {
            IsSatisfied = _predicate();
        }

        #endregion
    }
}