using System;

namespace Woosh.Signals
{
    /// <summary>
    /// An interface for defining a place where callbacks are registered too.
    /// </summary>
    public interface IDispatchTable : IDisposable
    {
        /// <inheritdoc cref="Dispatcher.Registered"/>
        event Action<RegisteredEventType> Registered;

        /// <inheritdoc cref="Dispatcher.Count"/>
        int Count(Type type);

        /// <inheritdoc cref="Dispatcher.Register"/>
        void Register(Type type, Delegate func);

        /// <inheritdoc cref="Dispatcher.Unregister"/>
        void Unregister(Type type, Delegate func);
    }
}
