#if SANDBOX
using Sandbox;

namespace Woosh.Signals;

public abstract class ObservableModelEntity : ModelEntity, IObservableEntity
{
    private IDispatcher m_Events;
    public IDispatcher Events => m_Events ??= Dispatcher.CreateForEntity(this);

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
}
#endif
