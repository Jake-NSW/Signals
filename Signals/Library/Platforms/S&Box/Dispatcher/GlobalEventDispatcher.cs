#if SANDBOX
using System.Threading.Tasks;
using Sandbox;

namespace Woosh.Signals;

public sealed class GlobalEventDispatcher : IDispatchExecutor
{
    public static GlobalEventDispatcher Instance { get; } = new GlobalEventDispatcher();

    private GlobalEventDispatcher() { }

    public bool Run<T>(Event<T> data, Propagation propagation) where T : struct, ISignal
    {
        Event.Run(typeof(T).FullName, data.Data);
        return true;
    }

    public Task RunAsync<T>(Event<T> data, Propagation propagation) where T : struct, ISignal
    {
        // Not Supported with s&box events
        return Task.CompletedTask;
    }
}
#endif
