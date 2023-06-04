#if UNITY
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Woosh.Signals
{
    public abstract class ObservableMonoBehaviour : MonoBehaviour
    {
        private InternalMonoDispatcher m_Observable;

        private IDispatcher Events
        {
            get
            {
                if (m_Observable != null)
                    return m_Observable.Events;

                m_Observable = TryGetComponent<InternalMonoDispatcher>(out var comp) ? comp : gameObject.AddComponent<InternalMonoDispatcher>();
                return m_Observable.Events;
            }
        }

        protected virtual void OnAttached() { }
        protected virtual void OnDetached() { }

        // Registry

        public IDispatchTable Registry
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Events;
        }

        private RegisteredEventType[] m_Library;

        protected virtual void OnAutoRegister()
        {
            m_Library ??= ObservableUtility.AutoMethodsFromType(GetType(), this).ToArray();

            foreach (var method in m_Library)
            {
                if (method.Event != null)
                    Registry.Register(method.Event, method.Delegate);
            }
        }

        protected virtual void OnAutoUnregister()
        {
            if (m_Library == null)
                return;

            foreach (var method in m_Library)
            {
                if (method.Event != null)
                    Registry.Unregister(method.Event, method.Delegate);
            }
        }

        private void OnEnable()
        {
            OnAutoRegister();
            OnAttached();
        }

        private void OnDisable()
        {
            OnAutoUnregister();
            OnDetached();
        }

        private void OnDestroy()
        {
            OnAutoUnregister();
            OnDetached();
        }
    }
}

#endif