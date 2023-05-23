#if UNITY
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Woosh.Signals
{
    public static class ObservableMonoUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDispatchTable Registry(this GameObject go)
        {
            return go.GetComponent<InternalMonoDispatcher>().Events;
        }
    }

    internal sealed class InternalMonoDispatcher : MonoBehaviour
    {
        public IDispatcher Events { get; } = new Dispatcher();

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
