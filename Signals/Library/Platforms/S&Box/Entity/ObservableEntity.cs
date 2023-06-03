#if SANDBOX
using Sandbox;

namespace Woosh.Signals;

public abstract class ObservableEntity : Entity, IObservableEntity
{
    private IDispatcher m_Events;
    public IDispatcher Events => m_Events ??= Dispatcher.CreateForEntity(this);

    // Components

    protected override void OnComponentAdded(EntityComponent component)
    {
        base.OnComponentAdded(component);
        Events.Run(new ComponentAdded(component));
    }

    protected override void OnComponentRemoved(EntityComponent component)
    {
        base.OnComponentRemoved(component);
        Events.Run(new ComponentRemoved(component));
    }
}
#endif
