
using System.Threading.Tasks;
#if UNITY
using UnityEngine.Scripting;
#endif

namespace Woosh.Signals
{
#if UNITY
    [Preserve]
#endif
    public delegate void StructCallback<T>(Event<T> evt) where T : struct, ISignal;
    
#if UNITY
    [Preserve]
#endif
    public delegate void RefStructCallback<T>(ref Event<T> evt) where T : struct, ISignal;
    
#if UNITY
    [Preserve]
#endif
    public delegate Task AsyncStructCallback<T>(Event<T> evt) where T : struct, ISignal;
    
#if UNITY
    [Preserve]
#endif
    public delegate Task RefAsyncStructCallback<T>(ref Event<T> evt) where T : struct, ISignal;
}