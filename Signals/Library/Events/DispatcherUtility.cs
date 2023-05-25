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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register(this IDispatchTable table, RegisteredEventType evt)
        {
            if (evt.Event != null)
                table.Register(evt.Event, evt.Delegate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister<T>(this IDispatchTable dispatcher, StructCallback<T> callback) where T : struct, ISignal
        {
            dispatcher.Unregister(typeof(T), callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister(this IDispatchTable table, RegisteredEventType evt)
        {
            table.Unregister(evt.Event, evt.Delegate);
        }
    }
}