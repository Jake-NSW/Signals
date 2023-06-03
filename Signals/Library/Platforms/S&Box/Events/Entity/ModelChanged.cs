#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly record struct EntityTakenDamage(Entity Entity, DamageInfo LastDamageInfo) : ISignal { }

    public readonly record struct ModelChanged(Model Model) : ISignal { }
}
#endif
