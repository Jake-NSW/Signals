using System;
using System.Collections.Generic;

namespace Woosh.Signals
{
    [Flags]
    public enum Propagation { Trickle, Bubble }

    public sealed class Dispatcher : IDispatcher, IDisposable
    {
        public IObservable Attached { get; }

        // Registry

        private readonly Dictionary<Type, HashSet<Delegate>> m_Registry;

        public Dispatcher(IObservable attached = null)
        {
            Attached = attached;
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
#if UNITY
                if (evt.Target == null && !evt.Method.IsStatic)
                    continue;
#endif
                try
                {
                    (evt as Action)?.Invoke();
                    (evt as StructCallback<T>)?.Invoke(passthrough);
                }
                catch (Exception e)
                {
                    // The show must go on..
#if SANDBOX
                    Log.Error(e);
#elif UNITY
                    UnityEngine.Debug.LogException(e);
#endif
                    continue;
                }
            }

            if (Attached == null)
                return;

            // Propagate
            foreach (var dispatcher in Attached.OnTrickleEvent())
            {
                dispatcher.Run(item, from);
            }

            if (Attached.OnBubbleEvent() is { } bubble)
            {
                bubble.Run(item, from);
            }
        }

        public void Unregister(Type type, Delegate callback)
        {
            if (m_Registry.TryGetValue(type, out var stack))
            {
                stack.Remove(callback);
            }
        }

        public event Action<RegisteredEventType> Registered;

        public int Count(Type type)
        {
            return m_Registry.TryGetValue(type, out var items) ? items.Count : 0;
        }

        public void Register(Type type, Delegate callback)
        {
            Registered?.Invoke(new RegisteredEventType(type, callback));

            if (m_Registry.TryGetValue(type, out var data))
            {
                data.Add(callback);
                return;
            }

            m_Registry.Add(type, new HashSet<Delegate> { callback });
        }
    }
}

public class TEst { }
