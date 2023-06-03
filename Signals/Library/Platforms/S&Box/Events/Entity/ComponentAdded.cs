#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly record struct ComponentAdded(EntityComponent Component) : ISignal;
}

#endif
