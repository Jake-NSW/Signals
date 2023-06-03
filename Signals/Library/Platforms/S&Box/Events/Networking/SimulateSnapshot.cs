#if SANDBOX
using Sandbox;

namespace Woosh.Signals
{
    public readonly struct SimulateSnapshot : ISignal
    {
        public IClient Client { get; }

        public SimulateSnapshot(IClient client)
        {
            Client = client;
        }
    }
}
#endif
