using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Woosh.Signals
{
    // Should this be in a namespace? It's annoying have to import it everywhere.

    public static partial class DispatcherUtility
    {
        private static Type[] m_SignalLibrary;

        /// <summary>
        /// Gets all signal types in the current domain. This is cached.
        /// </summary>
        public static Type[] FindAllSignalTypes()
        {
            if (m_SignalLibrary != null)
                return m_SignalLibrary;

            var asm = typeof(ISignal).Assembly;
            m_SignalLibrary = AppDomain.CurrentDomain.GetAssemblies()
                .Where(e => e.GetReferencedAssemblies().Any(e => e.FullName == asm.FullName) || e == asm)
                .SelectMany(e => e.GetTypes().Where(e => e.GetInterfaces().Any(e => e == typeof(ISignal))))
                .ToArray();

            return m_SignalLibrary;
        }

        // Utility

        /// <summary>
        /// Checks if there are any callbacks registered for the given signal type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<T>(this IDispatchTable table) where T : struct, ISignal
        {
            return table.Count<T>() > 0;
        }

        /// <inheritdoc cref="Dispatcher.Count"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this IDispatchTable table) where T : struct, ISignal
        {
            return table.Count(typeof(T));
        }

        /// <inheritdoc cref="Dispatcher.Run{T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(this IDispatchExecutor table, T data, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal
        {
            table.Run(new Event<T>(data, from), propagation);
        }

        /// <inheritdoc cref="Dispatcher.RunAsync{T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RunAsync<T>(this IDispatchExecutor table, T data, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal
        {
            return table.RunAsync(new Event<T>(data, from), propagation);
        }

        /// <inheritdoc cref="Dispatcher.Run{T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(this IDispatchExecutor table, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal
        {
            table.Run<T>(default, propagation);
        }

        /// <inheritdoc cref="Dispatcher.RunAsync{T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RunAsync<T>(this IDispatchExecutor table, Propagation propagation = Propagation.None, object from = null) where T : struct, ISignal
        {
            return table.RunAsync<T>(default, propagation);
        }

        /// <summary>
        /// Records all callbacks invoked by the given dispatcher. This is used to more easily unregister all callbacks registered.
        /// Because of this it allows you to use anon functions for callbacks.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DispatchTableRecorder Record(this IDispatchTable table)
        {
            return new DispatchTableRecorder(table);
        }

        // Register

        /// <inheritdoc cref="Dispatcher.Register"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<T>(this IDispatchTable dispatcher, StructCallback<T> callback) where T : struct, ISignal
        {
            dispatcher.Register(typeof(T), callback);
        }

        /// <inheritdoc cref="Dispatcher.Register"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<T>(this IDispatchTable dispatcher, AsyncStructCallback<T> callback) where T : struct, ISignal
        {
            dispatcher.Register(typeof(T), callback);
        }

        /// <inheritdoc cref="Dispatcher.Register"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<T>(this IDispatchTable dispatcher, Action callback) where T : struct, ISignal
        {
            dispatcher.Register(typeof(T), callback);
        }

        /// <inheritdoc cref="Dispatcher.Register"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register(this IDispatchTable table, RegisteredEventType evt)
        {
            if (evt.Event != null)
                table.Register(evt.Event, evt.Delegate);
        }

        // Unregister

        /// <inheritdoc cref="Dispatcher.Unregister"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister<T>(this IDispatchTable dispatcher, StructCallback<T> callback) where T : struct, ISignal
        {
            dispatcher.Unregister(typeof(T), callback);
        }

        /// <inheritdoc cref="Dispatcher.Register"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister<T>(this IDispatchTable dispatcher, AsyncStructCallback<T> callback) where T : struct, ISignal
        {
            dispatcher.Unregister(typeof(T), callback);
        }

        /// <inheritdoc cref="Dispatcher.Unregister"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister<T>(this IDispatchTable dispatcher, Action callback) where T : struct, ISignal
        {
            dispatcher.Unregister(typeof(T), callback);
        }

        /// <inheritdoc cref="Dispatcher.Unregister"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unregister(this IDispatchTable table, RegisteredEventType evt)
        {
            table.Unregister(evt.Event, evt.Delegate);
        }
    }
}
