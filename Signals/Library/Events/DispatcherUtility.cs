using System;
using System.Runtime.CompilerServices;

namespace Woosh.Signals
{
    public static partial class DispatcherUtility
    {
        // Utility

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<T>(this IDispatchTable table) where T : struct, ISignal
        {
            return table.Count<T>() > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this IDispatchTable table) where T : struct, ISignal
        {
            return table.Count(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(this IDispatchExecutor table, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal
        {
            table.Run<T>(data: default, propagation, from);
        }

        public static DispatchTableRecorder Record(this IDispatchTable table)
        {
            return new DispatchTableRecorder(table);
        }

        // Register

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<T>(this IDispatchTable dispatcher, StructCallback<T> callback) where T : struct, ISignal
        {
            dispatcher.Register(typeof(T), callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<T>(this IDispatchTable dispatcher, Action callback) where T : struct, ISignal
        {
            dispatcher.Register(typeof(T), callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register(this IDispatchTable table, RegisteredEventType evt)
        {
            if (evt.Event != null)
                table.Register(evt.Event, evt.Delegate);
        }

        // Unregister

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister<T>(this IDispatchTable dispatcher, StructCallback<T> callback) where T : struct, ISignal
        {
            dispatcher.Unregister(typeof(T), callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister<T>(this IDispatchTable dispatcher, Action callback) where T : struct, ISignal
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
