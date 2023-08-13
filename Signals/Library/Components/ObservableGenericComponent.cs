using System.Collections.Generic;

namespace Woosh.Signals
{
    public abstract class ObservableGenericComponent<T> : IComponent<T> where T : class, IObservable
    {
        public IDispatcher Events => Attached.Events;

        public T Attached { get; set; }
        LinkedListNode<IComponent<T>> IComponent<T>.Node { get; set; }

        public virtual bool Attachable(T item)
        {
            return true;
        }

        void IComponent<T>.OnAttached()
        {
            OnAutoRegister();
            OnAttached();
        }

        void IComponent<T>.OnDetached()
        {
            OnDetached();
            OnAutoUnregister();
        }

        protected virtual void OnAttached() { }
        protected virtual void OnDetached() { }

        // Registry

        private RegisteredEventType[] m_Library;

        protected virtual void OnAutoRegister()
        {
            if (m_Library == null)
            {
                AppendAutoMethods();
            }

            foreach (var method in m_Library!)
            {
                if (method.Event != null)
                    Events.Register(method.Event, method.Delegate);
            }
        }

        private void AppendAutoMethods()
        {
            m_Library = ObservableUtility.AutoMethodsFromType(this).ToArray();
        }

        protected virtual void OnAutoUnregister()
        {
            foreach (var method in m_Library!)
            {
                if (method.Event != null)
                    Events.Unregister(method.Event, method.Delegate);
            }
        }
    }
}
