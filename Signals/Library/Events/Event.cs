namespace Woosh.Signals
{
#if !SANDBOX
    /// <summary>
    /// Global event dispatcher. Used to dispatch events to every registered or static function. Incredibly useful for global state events
    /// such as Fullscreen Changing, Resolution Changing, Quit, etc.
    /// </summary>
    public static class Event
    {
        /// <summary>
        /// The executor that is used to run the events. This allows you to pass through the global dispatcher to instanced dispatchers
        /// execution chain. This is readonly to prevent the executor from being modified.
        /// </summary>
        public static IDispatchExecutor Executor => m_Dispatcher;

        private readonly static GlobalDispatcher m_Dispatcher;

        static Event()
        {
            m_Dispatcher = new GlobalDispatcher();
        }

        /// <inheritdoc cref="Dispatcher.Run{T}"/>
        public static bool Run<T>(T signal) where T : struct, ISignal
        {
            var evt = new Event<T>(signal);
            return Run(ref evt);
        }

        /// <inheritdoc cref="Dispatcher.Run{T}"/>
        public static bool Run<T>(ref Event<T> signal) where T : struct, ISignal
        {
            return m_Dispatcher.Run(ref signal, Propagation.None);
        }

        /// <summary>
        /// Registers an object to receive global callbacks. This will register it as a weak-ref, so you don't have to worry about unregistering
        /// it. It will be automatically removed when it is garbage collected. It will use the [Listen(Global = true)] attribute to determine
        /// the methods to register.
        /// </summary>
        public static void Register<T>(T item) where T : class
        {
            // Whys this parameter not just object?

            m_Dispatcher.Register(item);
        }
    }
#endif

    /// <summary>
    /// An event is a struct that is passed through the dispatcher. It is used to pass data to callbacks. It also contains methods to stop
    /// and consume the event. This is the backbone of the signal system.
    /// </summary>
    public ref struct Event<T> where T : struct, ISignal
    {
        /// <summary>
        /// The signal that is being passed through the dispatcher. This is the object that contains the data that is being passed to the
        /// callbacks. Which is defined by the <see cref="ISignal"/> interface. This is readonly to prevent the signal from being modified.
        /// </summary>
        public readonly T Signal;

        /// <summary>
        /// Constructs a new event with the given signal. This is used to pass data to the callbacks. Which is defined by the
        /// <see cref="ISignal"/> interface. This is readonly to prevent the signal from being modified. This also sets the stopping and
        /// consumed flags to false that way the event can propagate up and down the chain.
        /// <param name="signal"></param>
        /// </summary>
        public Event(T signal)
        {
            Signal = signal;

            Stopping = false;
            Consumed = false;
        }

        // These only work if you ref the struct as a parameter. c# sucks

        internal bool Stopping;

        /// <summary>
        /// Stop will stop this event from propagating up or down the chain. But will still execute the callbacks on the current dispatcher.
        /// This is useful for input.
        /// </summary>
        public void Stop()
        {
            Stopping = true;
        }

        internal bool Consumed;

        /// <summary>
        /// Consumes this event. This will stop the event from propagating up or down the chain. And will not execute the callbacks on the
        /// current dispatcher. This will also return false in the <see cref="Dispatcher.Run{T}"/> method.
        /// </summary>
        public void Consume()
        {
            Consumed = true;
        }

        public static implicit operator T(Event<T> item) => item.Signal;
    }
}
