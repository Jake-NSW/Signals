#if UNITY
using UnityEngine;

namespace Woosh.Signals
{
    internal sealed class InternalMonoDispatcher : MonoBehaviour
    {
        public IDispatcher Events { get; } = new Dispatcher();

        private void Awake()
        {
            hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
        }

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
