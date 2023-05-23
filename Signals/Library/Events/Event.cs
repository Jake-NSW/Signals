#if SANDBOX
using Sandbox;
#endif

namespace Woosh.Signals
{
    public readonly ref struct Event<T> where T : struct, ISignal
    {
        public T Data { get; }
        public object From { get; }

        public Event(T data, object from = null)
        {
            Data = data;
            From = from;

#if SANDBOX
            IsPredicted = Prediction.Enabled && Prediction.CurrentHost != null;
#endif
        }
#if SANDBOX
        public bool IsPredicted { get; }

        public bool IsServer => Game.IsServer;
        public bool IsClient => Game.IsClient;
#endif
    }
}
