using System;

namespace Woosh.Signals
{
    public interface IDispatchTable
    {
        event Action<RegisteredEventType> Registered;

        void Register(Type type, Delegate func);
        void Unregister(Type type, Delegate func);
    }
}
