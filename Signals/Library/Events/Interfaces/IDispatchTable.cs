using System;

namespace Woosh.Signals
{
    public interface IDispatchTable
    {
        event Action<RegisteredEventType> Registered;
        
        int Count(Type type);

        void Register(Type type, Delegate func);
        void Unregister(Type type, Delegate func);
    }
}
