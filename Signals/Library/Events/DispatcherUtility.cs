using System.Runtime.CompilerServices;

namespace Woosh.Signals
{
    public static class DispatcherUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<T>(this IDispatchTable dispatcher, StructCallback<T> callback) where T : struct, ISignal
        {
            dispatcher.Register(typeof(T), callback);
        }
    }
}
