#if SANDBOX
using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.UI;

namespace Woosh.Signals;

public abstract class EntityHudComponent<TEntity> : EntityHudComponent<RootPanel, TEntity> where TEntity : class, IObservableEntity { }

public abstract class EntityHudComponent<T, TEntity> : ObservableEntityComponent where T : RootPanel, new() where TEntity : class, IObservableEntity
{
    public new TEntity Entity => base.Entity as TEntity;

    public override bool CanAddToEntity(Entity entity)
    {
        return entity is TEntity;
    }

    protected override void OnActivate()
    {
        if (Game.IsClient)
        {
            // Make sure we have a root panel handler
            _ = Handler;

            // Only Register Events on Client
            base.OnActivate();
        }

        if (Game.IsClient && Game.LocalPawn == Entity)
        {
            CreateUI();
        }
    }

    private HudComponentHandler<T> Handler
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Entity.Components.GetOrCreate<HudComponentHandler<T>>();
    }

    private Panel m_Root;

    private void CreateUI()
    {
        m_Root = new Panel
        {
            ElementName = GetType().Name,
            Style =
            {
                Position = PositionMode.Absolute,
                Width = Length.Percent(100),
                Height = Length.Percent(100)
            }
        };

        OnCreateUI(m_Root);
        Handler.Panel.AddChild(m_Root);
    }

    [Listen]
    private void OnPawnPossessed(Event<EntityPossessed> signal)
    {
        m_Root?.Delete();

        if (signal.Data.Client != Game.LocalClient)
            return;

        CreateUI();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        m_Root?.Delete();
    }

    protected virtual void OnCreateUI(Panel root) { }
}

#endif
