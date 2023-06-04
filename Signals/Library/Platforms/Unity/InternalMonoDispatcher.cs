#if UNITY
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Woosh.Signals
{
    internal sealed class InternalMonoDispatcher : MonoBehaviour, IObservable
    {
        private IDispatcher m_Dispatcher;
        public IDispatcher Events => m_Dispatcher ??= Dispatcher.CreateForGameObject(gameObject);

        private void Awake()
        {
            hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
        }

        // Collision

        private void OnCollisionEnter(Collision other)
        {
            Events.Run(new CollisionEnter(other));
        }

        private void OnCollisionStay(Collision other)
        {
            Events.Run(new CollisionStay(other));
        }

        private void OnCollisionExit(Collision other)
        {
            Events.Run(new CollisionExit(other));
        }

    }
}
#endif
