using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
#if SANDBOX
using Sandbox;
#endif

namespace Woosh.Signals
{
#if !SANDBOX
    public static class Event
    {
        public static IDispatchExecutor Executor => m_Dispatcher;
        private readonly static GlobalDispatcher m_Dispatcher;

        static Event()
        {
            m_Dispatcher = new GlobalDispatcher();
        }

        public static void Run<T>(Event<T> signal) where T : struct, ISignal
        {
            m_Dispatcher.Run(signal, Propagation.None);
        }

         public static void Register<T>(T item) where T : class
        {
            m_Dispatcher.Register(item);
        }
    }
#endif

    public readonly ref struct Event<T> where T : struct, ISignal
    {
        public T Data { get; }
        public object From { get; }

        public static implicit operator T(Event<T> item) => item.Data;

        public Event(T data, object from = null)
        {
            Data = data;
            From = from;

#if SANDBOX
            IsPredicted = Prediction.Enabled && Prediction.CurrentHost != null;
#endif
        }
#if SANDBOX
        public bool IsPredicted { get; }

        public bool IsServer => Game.IsServer;
        public bool IsClient => Game.IsClient;
#endif
    }
}
