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

            if (Game.LocalPawn == Entity)
                CreateUI();
        }
    }

    private HudComponentHandler<T> Handler
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Entity.Components.GetOrCreate<HudComponentHandler<T>>();
    }

    public Panel Panel => m_Root;
    private Panel m_Root;

    protected Panel CreateFullscreenPanel()
    {
        return new Panel()
        {
            Style =
            {
                Position = PositionMode.Absolute, Width = Length.Percent(100), Height = Length.Percent(100)
            }
        };
    }

    private void CreateUI()
    {
        m_Root = OnCreateUI();

        if (m_Root == null)
        {
            Log.Error($"No Root was returned from {GetType().Name}.OnCreateUI()");
            return;
        }

        m_Root.ElementName = GetType().Name;
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
        if (Game.IsClient)
        {
            base.OnDeactivate();
            m_Root?.Delete();
        }
    }

    protected virtual Panel OnCreateUI() { return null; }
}

#endif
