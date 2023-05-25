#if UNITY
using UnityEngine.Scripting;
#endif

namespace Woosh.Signals
{
#if UNITY
    [Preserve]
#endif
    public delegate void StructCallback<T>(Event<T> evt) where T : struct, ISignal;
}