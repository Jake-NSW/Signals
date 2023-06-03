#if SANDBOX
using Sandbox;

namespace Woosh.Signals;

public abstract class ObservableClientComponent : ObservableEntityComponent
{
    protected override IDispatcher Events => Entity.Components.GetOrCreate<ClientEventDispatcher>().Events;

    public new IClient Entity => base.Entity as IClient;

    public override bool CanAddToEntity(Entity entity)
    {
        return entity is IClient;
    }
}
#endif
