#if UNITY
using System;
using UnityEngine;

namespace Woosh.Signals
{
    public abstract class ObservableMonoBehaviour : MonoBehaviour
    {
        public IDispatcher Events { get; }

        protected virtual void OnAttached() { }
        protected virtual void OnDetached() { }

        // Registry

        private (Type Event, Delegate Delegate)[] m_Library;

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
            m_Library = ObservableUtility.AutoMethodsFromType(GetType(), this).ToArray();
        }

        protected virtual void OnAutoUnregister()
        {
            if (m_Library == null)
                return;

            foreach (var method in m_Library)
            {
                if (method.Event != null)
                    Events.Unregister(method.Event, method.Delegate);
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
