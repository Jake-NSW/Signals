#if SANDBOX
using Sandbox;

namespace Woosh.Signals;

public readonly record struct PhysicsCollision(CollisionEventData Collision) : ISignal;
#endif
