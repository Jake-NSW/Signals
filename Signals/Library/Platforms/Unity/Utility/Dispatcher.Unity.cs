using UnityEngine;

namespace Woosh.Signals
{
    partial class Dispatcher
    {
        public static IDispatcher FindForGameObject(GameObject gameObject, bool add = true)
        {
            if (gameObject.TryGetComponent<InternalMonoDispatcher>(out var observable))
                return observable.Events;

            return add ? gameObject.AddComponent<InternalMonoDispatcher>().Events : null;
        }

        public static IDispatcher CreateForGameObject(GameObject gameObject)
        {
            return new Dispatcher(
                attached: gameObject,
                bubble: static attached =>
                {
                    var go = (GameObject)attached;
                    return FindForGameObject(go.transform.parent.gameObject, false);
                },
                trickle: static attached =>
                {
                    var go = (GameObject)attached;
                    var transform = go.transform;
                    var observables = transform.GetComponentsInChildren<InternalMonoDispatcher>();

                    var dispatchers = new IDispatcher[observables.Length];

                    for (var i = 0; i < observables.Length; i++)
                    {
                        dispatchers[i] = observables[i].Events;
                    }

                    return dispatchers;
                }
            );
        }
    }
}
