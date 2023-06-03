#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly record struct ComponentRemoved(EntityComponent Component) : ISignal;
}

#endif
