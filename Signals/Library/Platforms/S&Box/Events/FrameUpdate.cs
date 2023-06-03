#if SANDBOX
namespace Woosh.Signals
{
    public readonly struct FrameUpdate : ISignal
    {
        public float Delta { get; }

        public FrameUpdate(float delta)
        {
            Delta = delta;
        }
    }
}
#endif
