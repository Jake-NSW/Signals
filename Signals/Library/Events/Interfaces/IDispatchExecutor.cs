using System.Threading.Tasks;

namespace Woosh.Signals
{
    public interface IDispatchExecutor
    {
        /// <inheritdoc cref="Dispatcher.Run{T}"/>
        bool Run<T>(T data, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal;

        /// <inheritdoc cref="Dispatcher.RunAsync{T}"/>
        Task RunAsync<T>(T data, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal;
    }
}
