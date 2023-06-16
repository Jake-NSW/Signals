﻿#if SANDBOX
using Sandbox;
using Sandbox.UI;

namespace Woosh.Signals;

public abstract class EntityHudComponent<TPanel, TEntity> : EntityHudComponent<TPanel> where TPanel : RootPanel, new() where TEntity : class, IObservableEntity
{
    public new TEntity Entity => base.Entity as TEntity;
    protected Entity UnderlyingEntity => base.Entity;

    public override bool CanAddToEntity(Entity entity) => entity is TEntity;
}

public abstract class EntityHudComponent<T> : ObservableEntityComponent where T : RootPanel, new()
{
    protected override void OnActivate()
    {
        if (Game.IsClient)
        {
            // Only Register Events on Client
            base.OnActivate();
        }

        if (Game.IsClient && Game.LocalPawn == Entity)
        {
            CreateUI();
        }
    }

    private void CreateUI()
    {
        OnCreateUI(m_Panel = new T());
#if DEBUG
        m_Panel.ElementName = $"{GetType().Name} / {Entity.Name}";
#endif
    }

    public T Panel => m_Panel;
    private T m_Panel;

    [Listen]
    private void OnPawnPossessed(Event<EntityPossessed> signal)
    {
        // Delete Old UI
        m_Panel?.Delete();

        if (signal.Data.Client != Game.LocalClient)
            return;

        CreateUI();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        m_Panel?.Delete();
    }

    protected virtual void OnCreateUI(T root) { }
}

#endif
