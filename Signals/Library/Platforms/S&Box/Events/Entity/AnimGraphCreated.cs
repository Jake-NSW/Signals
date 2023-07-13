using Sandbox;

namespace Woosh.Signals;

public readonly record struct AnimGraphCreated(AnimatedEntity Entity) : ISignal;