#if SANDBOX
using Sandbox;

namespace Woosh.Signals;

public abstract class ObservableEntity : Entity, IObservable
{
    private IDispatcher m_Dispatcher;
    private RegisteredEventType[] m_Events;

    public IDispatcher Events
    {
        get
        {
            if (m_Dispatcher != null)
                return m_Dispatcher;

            Dispatcher.CreateForEntity(this, static entity => entity.OnAutoRegister(), ref m_Dispatcher, ref m_Events);
            return m_Dispatcher;
        }
    }

    protected virtual void OnAutoRegister()
    {
        ObservableUtility.AutoRegisterEvents(this, Events);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Dispatcher.DisposeForEntity(this, ref m_Dispatcher, ref m_Events);
    }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
        Events.Run(new SimulateSnapshot(cl));
    }

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
