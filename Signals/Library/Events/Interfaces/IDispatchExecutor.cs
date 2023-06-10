using System.Threading.Tasks;

namespace Woosh.Signals
{
    public interface IDispatchExecutor
    {
        /// <inheritdoc cref="Dispatcher.Run{T}"/>
        bool Run<T>(Event<T> data, Propagation propagation = Propagation.None) where T : struct, ISignal;

        /// <inheritdoc cref="Dispatcher.RunAsync{T}"/>
        Task RunAsync<T>(Event<T> data, Propagation propagation = Propagation.None) where T : struct, ISignal;
    }
}
