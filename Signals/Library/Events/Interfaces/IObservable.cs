namespace Woosh.Signals
{
    public interface IObservable
    {
        public IDispatcher Events { get; }
    }
}
