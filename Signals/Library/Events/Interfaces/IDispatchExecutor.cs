namespace Woosh.Signals
{
    public interface IDispatchExecutor
    {
        void Run<T>(T data, object from = null) where T : struct, ISignal;
    }
}
