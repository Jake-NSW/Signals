using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Woosh.Signals
{
    [Flags]
    public enum Propagation { None, Trickle, Bubble, Both = Trickle | Bubble }

    public delegate IDispatcher BubbleEvent(object attached);

    public delegate IDispatcher[] TrickleEvent(object attached);

    /// <summary>
    /// The Dispatcher is responsible for dispatching events to all registered callbacks. It will also propagate the event to the
    /// parent and children of what ever it is attached to. Which is done by using the propagation flags.
    /// </summary>
    public sealed partial class Dispatcher : IDispatcher
    {
        /// <summary>
        /// Provides a way of using a dispatcher that does not do anything. This is useful for when you want to have a
        /// observable "Component" but its attached to something that isn't observable.
        /// </summary>
        public static IDispatcher Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BlankDispatcher.Instance;
        }

        /// <summary>
        /// The Object that this dispatcher has been instantiated for. This is used to determine the parent and children of this
        /// dispatcher via its Bubble and Trickle callbacks defined in the constructor.
        /// </summary>
        public object Attached { get; }

        // Registry

        private readonly BubbleEvent m_Bubble;
        private readonly TrickleEvent m_Trickle;

        private readonly Dictionary<Type, HashSet<Delegate>> m_Registry;

        /// <summary>
        /// Creates a basic dispatcher that doesn't propagate events.
        /// </summary>
        public Dispatcher() : this(null, null, null) { }


        /// <summary>
        /// Creates a dispatcher that has event propagation, that will use the bubble and trickle callbacks to determine the parent
        /// and children dispatchers. 
        /// </summary>
        public Dispatcher(object attached, BubbleEvent bubble, TrickleEvent trickle)
        {
            Attached = attached;

            m_Bubble = bubble;
            m_Trickle = trickle;

            m_Registry = new Dictionary<Type, HashSet<Delegate>>();
        }

        /// <summary>
        /// Removes all the events from this dispatchers registry
        /// </summary>
        public void Dispose()
        {
            m_Registry.Clear();
        }

        // Dispatch

        /// <summary>
        /// Runs a new event through the dispatcher, which will then be dispatched to all registered callbacks. An event can be consumed
        /// by one of its callbacks, which will stop the event from being dispatched to any other callbacks. This is useful for being
        /// able to stop an event from propagating to the parent or children. Will not run async callbacks, as it doesn't make sense.
        /// </summary>
        public bool Run<T>(Event<T> item, Propagation propagation = Propagation.None) where T : struct, ISignal
        {
            // Dispatch to our callbacks
            if (m_Registry.TryGetValue(typeof(T), out var stack))
            {
                // This should be made by the caller, but we'll do it here for now. This will allocate a new event on every frame when we 
                // are propagating events. This is not ideal, but it's not a huge deal either.

                foreach (var evt in stack)
                {
                    try
                    {
                        (evt as Action)?.Invoke();
                        (evt as StructCallback<T>)?.Invoke(item);
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
            }

            if (Attached == null || propagation == Propagation.None)
                return true;

            if (FastHasFlag(propagation, Propagation.Trickle))
            {
                // Go to each child and propagate to them
                var dispatchers = m_Trickle.Invoke(Attached);
                foreach (var dispatcher in dispatchers)
                {
                    if (dispatcher?.Run(item, Propagation.Trickle) == false)
                    {
                        return false;
                    }
                }
            }

            if (FastHasFlag(propagation, Propagation.Bubble))
            {
                // Go to the parent and bubble up the event
                if (m_Bubble.Invoke(Attached) is { } bubble)
                {
                    if (!bubble.Run(item, Propagation.Bubble))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Runs a new event through the dispatcher, which will then be dispatched to all registered callbacks. Events can't be consumed
        /// as this is an async dispatcher. This will also run normal callbacks, but it will not wait for them to finish as they are not
        /// async. Only use RunAsync if you know there is a callback that uses a task. As using this can be considered a performance hit.
        /// </summary>
        public Task RunAsync<T>(Event<T> data, Propagation propagation = Propagation.None) where T : struct, ISignal
        {
            var has = m_Registry.TryGetValue(typeof(T), out var stack);

            // This is probably dumb as we are setting the capacity count to the same as the stack, but they might not have any async
            // events in them... ?
            var tasks = new List<Task>(stack?.Count ?? 0);

            // Add tasks from this dispatcher
            if (has)
            {
                foreach (var evt in stack)
                {
                    try
                    {
                        (evt as Action)?.Invoke();
                        (evt as StructCallback<T>)?.Invoke(data);

                        if (evt is AsyncStructCallback<T> cb)
                            tasks.Add(cb.Invoke(data));
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
            }

            // Add Propagation tasks
            if (Attached != null && propagation != Propagation.None)
            {
                if (FastHasFlag(propagation, Propagation.Trickle))
                {
                    // Go to each child and propagate to them
                    var dispatchers = m_Trickle.Invoke(Attached);
                    foreach (var dispatcher in dispatchers)
                    {
                        if (dispatcher != null)
                            tasks.Add(dispatcher.RunAsync(data, Propagation.Trickle));
                    }
                }

                if (FastHasFlag(propagation, Propagation.Bubble))
                {
                    // Go to the parent and bubble up the event
                    if (m_Bubble.Invoke(Attached) is { } bubble)
                    {
                        tasks.Add(bubble.RunAsync(data, Propagation.Bubble));
                    }
                }
            }

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Unregisters a callback from the dispatcher.
        /// </summary>
        public void Unregister(Type type, Delegate callback)
        {
            if (m_Registry.TryGetValue(type, out var stack))
            {
                stack.Remove(callback);
            }
        }

        public event Action<RegisteredEventType> Registered;

        /// <summary>
        /// Returns the count of all the callbacks registered for the given type.
        /// </summary>
        public int Count(Type type)
        {
            return m_Registry.TryGetValue(type, out var items) ? items.Count : 0;
        }

        /// <summary>
        /// Registers a new callback for the dispatcher
        /// </summary>
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool FastHasFlag(Propagation flags, Propagation flag)
        {
            return (flags & flag) != 0;
        }
    }
}
