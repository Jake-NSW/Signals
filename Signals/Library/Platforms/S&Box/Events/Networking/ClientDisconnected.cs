#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly record struct ClientDisconnected(IClient Client, NetworkDisconnectionReason Reason) : ISignal;
}

#endif
