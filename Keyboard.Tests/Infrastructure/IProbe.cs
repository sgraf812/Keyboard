namespace Keyboard.Tests.Infrastructure
{
    public interface IProbe
    {
        bool IsSatisfied { get; }
        string FailureMessage { get; }
        void Sample();
    }
}