#if UNITY
using UnityEngine;

namespace Woosh.Signals
{
    public readonly struct CollisionStay : ISignal
    {
        public Collision Collision { get; }

        public CollisionStay(Collision collision)
        {
            Collision = collision;
        }
    }
}
#endif