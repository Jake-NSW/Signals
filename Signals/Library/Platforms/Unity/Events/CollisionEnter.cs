#if UNITY
using UnityEngine;

namespace Woosh.Signals
{
    public readonly struct CollisionEnter : ISignal
    {
        public Collision Collision { get; }

        public CollisionEnter(Collision collision)
        {
            Collision = collision;
        }
    }
}
#endif