#if SANDBOX
using Sandbox;

namespace Woosh.Signals;

public abstract class ObservableAnimatedEntity : AnimatedEntity, IObservableEntity
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

    // Model

    public override void OnNewModel(Model model)
    {
        base.OnNewModel(model);
        Events.Run(new ModelChanged(model));
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

    // Damage

    public override void TakeDamage(DamageInfo info)
    {
        base.TakeDamage(info);
        Events.Run(new EntityTakenDamage(this, info));
    }

    protected override void OnPhysicsCollision(CollisionEventData eventData)
    {
        base.OnPhysicsCollision(eventData);
        Events.Run(new PhysicsCollision(eventData));
    }

    // Animator

    protected override void OnAnimGraphCreated()
    {
        base.OnAnimGraphCreated();
        Events.Run(new AnimGraphCreated(this));
    }
}
#endif
