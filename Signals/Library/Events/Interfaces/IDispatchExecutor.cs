using System.Threading.Tasks;

namespace Woosh.Signals
{
    /// <summary>
    /// An interface for defining where callbacks are executed from.
    /// </summary>
    public interface IDispatchExecutor
    {
        /// <inheritdoc cref="Dispatcher.Run{T}"/>
        bool Run<T>(Event<T> data, Propagation propagation) where T : struct, ISignal;

        /// <inheritdoc cref="Dispatcher.RunAsync{T}"/>
        Task RunAsync<T>(Event<T> data, Propagation propagation) where T : struct, ISignal;
    }
}
