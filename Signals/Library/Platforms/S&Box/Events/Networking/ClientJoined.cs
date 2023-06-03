#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly record struct ClientJoined(IClient Client) : ISignal;
}

#endif
