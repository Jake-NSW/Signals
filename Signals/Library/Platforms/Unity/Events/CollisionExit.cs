#if UNITY
using UnityEngine;

namespace Woosh.Signals
{
    public readonly struct CollisionExit : ISignal
    {
        public Collision Collision { get; }

        public CollisionExit(Collision collision)
        {
            Collision = collision;
        }
    }
}
#endif