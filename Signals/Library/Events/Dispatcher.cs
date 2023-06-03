using System;
using System.Collections.Generic;

namespace Woosh.Signals
{
    [Flags]
    public enum Propagation { None, Trickle, Bubble }

    public sealed partial class Dispatcher : IDispatcher
    {
        private readonly Func<object, IDispatcher> m_Bubble;
        private readonly Func<object, IEnumerable<IDispatcher>> m_Trickle;
        public object Attached { get; }

        // Registry

        private readonly Dictionary<Type, HashSet<Delegate>> m_Registry;

        public Dispatcher() : this(null, null, null) { }

        public Dispatcher(object attached, Func<object, IDispatcher> bubble, Func<object, IEnumerable<IDispatcher>> trickle)
        {
            Attached = attached;

            m_Bubble = bubble;
            m_Trickle = trickle;

            m_Registry = new Dictionary<Type, HashSet<Delegate>>();
        }

        public void Dispose()
        {
            m_Registry.Clear();
        }

        // Dispatch

        public bool Run<T>(T item, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal
        {
            if (!m_Registry.TryGetValue(typeof(T), out var stack))
            {
                return true;
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
                return true;

            // Propagate

            if (propagation.HasFlag(Propagation.Trickle))
            {
                // Go to each child and propagate to them
                foreach (var dispatcher in m_Trickle.Invoke(Attached))
                {
                    if (dispatcher?.Run(item, Propagation.Trickle, from) == false)
                        return false;
                }
            }

            if (propagation.HasFlag(Propagation.Bubble))
            {
                // Go to the parent and bubble up the event
                if (m_Bubble.Invoke(Attached) is { } bubble)
                {
                    if (!bubble.Run(item, Propagation.Bubble, from))
                        return false;
                }
            }

            return true;
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
