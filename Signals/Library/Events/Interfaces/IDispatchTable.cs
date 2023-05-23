using System;

namespace Woosh.Signals
{
    public interface IDispatchTable
    {
        void Register(Type type, Delegate func);
        void Unregister(Type type, Delegate func);
    }
}
