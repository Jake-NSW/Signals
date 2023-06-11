using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Woosh.Signals
{
    public sealed class GlobalDispatcher : IDispatchExecutor
    {
        private readonly Dictionary<Type, List<WeakReference>> m_References = new Dictionary<Type, List<WeakReference>>();

        public bool Run<T>(Event<T> data, Propagation propagation) where T : struct, ISignal
        {
            return true;
        }

        public Task RunAsync<T>(Event<T> data, Propagation propagation) where T : struct, ISignal
        {
            return Task.CompletedTask;
        }

        public void Register(object item)
        {
            var type = item.GetType();
            if (!m_References.TryGetValue(type, out var collection))
            {
                m_References.Add(type, collection = new List<WeakReference>());
            }
            collection.Add(new WeakReference(item));
        }
    }
}
