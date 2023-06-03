#if SANDBOX
using Sandbox;

namespace Woosh.Signals;

internal sealed class ClientEventDispatcher : EntityComponent, IObservable, ISingletonComponent
{
    public ClientEventDispatcher()
    {
        Events = Dispatcher.CreateForClient((IClient)Entity);
    }

    public IDispatcher Events { get; }

    public override bool CanAddToEntity(Entity entity)
    {
        return entity is IClient;
    }
}
#endif
