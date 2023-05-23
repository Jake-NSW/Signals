namespace Woosh.Signals
{
    public delegate void StructCallback<T>(Event<T> evt) where T : struct, ISignal;
}
