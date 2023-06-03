#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly record struct ModelChanged(Model Model) : ISignal
    {
        public Propagation Propagation => Propagation.Trickle;
    }
}
#endif
