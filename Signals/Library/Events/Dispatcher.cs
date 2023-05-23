using System;
using System.Collections.Generic;

namespace Woosh.Signals
{
    public sealed class Dispatcher : IDispatcher, IDisposable
    {
        // Registry

        private readonly Dictionary<Type, HashSet<Delegate>> m_Registry;

        public Dispatcher()
        {
            m_Registry = new Dictionary<Type, HashSet<Delegate>>();
        }

        public void Dispose()
        {
            m_Registry.Clear();
        }

        // Dispatch

        public void Run<T>(T item, object from = null) where T : struct, ISignal
        {
            if (!m_Registry.TryGetValue(typeof(T), out var stack))
            {
                return;
            }

            var passthrough = new Event<T>(item, from);
            foreach (var evt in stack)
            {
                (evt as StructCallback<T>)?.Invoke(passthrough);
            }
        }

        public void Unregister(Type type, Delegate callback)
        {
            if (m_Registry.TryGetValue(type, out var stack))
            {
                stack.Remove(callback);
            }
        }

        public void Register(Type type, Delegate callback)
        {
            if (m_Registry.TryGetValue(type, out var data))
            {
                data.Add(callback);
                return;
            }

            m_Registry.Add(type, new HashSet<Delegate> { callback });
        }
    }
}
