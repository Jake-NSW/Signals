#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly record struct EntityPossessed(IClient Client) : ISignal;
    public readonly record struct EntityUnPossessed() : ISignal;
}

#endif
