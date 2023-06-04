#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly record struct SimulateSnapshot(IClient Client) : ISignal;
}
#endif
