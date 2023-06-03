namespace Woosh.Signals
{
    public interface IDispatchExecutor
    {
        bool Run<T>(T data, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal;
    }
}
