#if SANDBOX
using Sandbox;

namespace Woosh.Signals;

public sealed class ListenFor<T> : EventAttribute
{
    public ListenFor() : base(typeof(T).FullName) { }
}
#endif