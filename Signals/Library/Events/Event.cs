namespace Woosh.Signals
{
    public readonly ref struct Event<T> where T : struct, ISignal
    {
        public T Data { get; }
        public object From { get; }

        public Event(T data, object from = null)
        {
            Data = data;
            From = from;
        }
    }
}
