#if SANDBOX
using Sandbox;
using Sandbox.UI;

namespace Woosh.Signals;

internal sealed class HudComponentHandler<T> : ObservableEntityComponent where T : RootPanel, new()
{
    public HudComponentHandler()
    {
        // Only allow the creation on the client
        Game.AssertClient();
    }

    private T m_Panel;

    public T Panel
    {
        get
        {
            if (m_Panel != null)
                return m_Panel;

            m_Panel = new T();
#if DEBUG
            m_Panel.ElementName = $"{GetType().Name} / {Entity.Name}";
#endif
            return m_Panel;
        }
    }

    protected override void OnAutoRegister()
    {
        Register<EntityUnPossessed>(OnUnPossessed);
    }

    protected override void OnDeactivate()
    {
        m_Panel?.Delete();
    }

    private void OnUnPossessed(Event<EntityUnPossessed> signal)
    {
        // Ignore...
        if (Game.LocalPawn == Entity)
            return;
        
        m_Panel?.Delete();
    }
}
#endif