﻿using System;
using System.Threading.Tasks;

namespace Woosh.Signals
{
    internal sealed class BlankDispatcher : IDispatcher
    {
        private static BlankDispatcher m_Instance;
        public static BlankDispatcher Instance => m_Instance ??= new BlankDispatcher();

        bool IDispatchExecutor.Run<T>(T data, Propagation propagation, object from) => true;

        Task IDispatchExecutor.RunAsync<T>(T data, Propagation propagation, object from) => Task.CompletedTask;

        void IDisposable.Dispose() { }

        event Action<RegisteredEventType> IDispatchTable.Registered
        {
            add { }
            remove { }
        }

        int IDispatchTable.Count(Type type) => 0;

        void IDispatchTable.Register(Type type, Delegate func) { }
        void IDispatchTable.Unregister(Type type, Delegate func) { }
    }
}
