#if UNITY
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Woosh.Signals
{
    public abstract class ObservableMonoBehaviour<T> : ObservableMonoBehaviour where T : struct, ITuple
    {
        private T? m_Tuple;

        protected T Components
        {
            get
            {
                if (m_Tuple.HasValue)
                    return m_Tuple.Value;

                var types = typeof(T).GetGenericArguments();
                var args = new object[types.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = GetComponent(types[i]);
                }

                m_Tuple = (T)Activator.CreateInstance(typeof(T), args);
                return m_Tuple.Value;
            }
        }
    }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Signal<T>(T item = default) where T : struct, ISignal
        {
            Events.Run(item, this);
        }

        // Registry

        public IDispatchTable Registry
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Events;
        }

        private (Type Event, Delegate Delegate)[] m_Library;

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
