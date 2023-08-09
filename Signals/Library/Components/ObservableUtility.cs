using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
#if SANDBOX
using Sandbox;
#endif

namespace Woosh.Signals
{
    public static class ObservableUtility
    {
#if !SANDBOX
        static ObservableUtility()
        {
            s_Libraries = new Dictionary<Type, (MethodInfo Method, Type Delegate, Type Event)[]>();
        }

        private readonly static Dictionary<Type, (MethodInfo Method, Type Delegate, Type Event)[]> s_Libraries;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<RegisteredEventType> AutoRegisterEvents(object instance, IDispatchTable table)
        {
            var events = AutoMethodsFromType(instance);
            foreach (var evt in events)
            {
                table.Register(evt);
            }

            return events;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<RegisteredEventType> AutoMethodsFromType(object instance)
        {
            return AutoMethodsFromType(instance.GetType(), instance);
        }

        public static Span<RegisteredEventType> AutoMethodsFromType(Type type, object instance)
        {
            if (!s_Libraries.TryGetValue(type, out var items))
            {
                AssignMethodToCache(type);
                items = s_Libraries[type];
            }

            Span<RegisteredEventType> library = new RegisteredEventType[items.Length];

            for (var i = 0; i < items.Length; i++)
            {
                var method = items[i].Method;
                var callback = method.CreateDelegate(items[i].Delegate, instance);
                library[i] = new RegisteredEventType(items[i].Event, callback);
            }

            return library;
        }

        private static Type CreateDelegateFromMethod(MethodInfo method, Type evt)
        {
            Type delegateType = null;

            var isRef = method.GetParameters()[0].ParameterType.IsByRef;
            if (method.ReturnType == typeof(Task))
            {
                delegateType = isRef ? typeof(RefAsyncStructCallback<>) : typeof(AsyncStructCallback<>);
            }

            delegateType ??= isRef ? typeof(RefStructCallback<>) : typeof(StructCallback<>);
            return delegateType.MakeGenericType(evt);
        }


        private static void AssignMethodToCache(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => !m.IsStatic && m.GetCustomAttribute<ListenAttribute>()?.Global == false)
                .ToArray();

            var cache = new (MethodInfo methodInfo, Type Delegate, Type Event)[methods.Length];

            for (var i = 0; i < methods.Length; i++)
            {
                var methodInfo = methods[i];
                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 1 && !parameters[0].ParameterType.IsGenericParameter)
                    throw new MethodAccessException("Invalid Auto Parameters");

                var parameterType = parameters[0].ParameterType.GetGenericArguments()[0];
                var delegateType = CreateDelegateFromMethod(methodInfo, parameterType);
                cache[i] = (methodInfo, delegateType, parameterType);
            }

            s_Libraries.Add(type, cache);
        }
#else
        static ObservableUtility()
        {
            m_Libraries = new Dictionary<Type, (MethodDescription Method, Type Delegate, Type Event)[]>();
        }

        private readonly static Dictionary<Type, (MethodDescription Method, Type Delegate, Type Event)[]> m_Libraries;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AutoRegisterEvents(object instance, IDispatchTable table)
        {
            foreach (var evt in AutoMethodsFromType(instance))
            {
                table.Register(evt);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegisteredEventType[] AutoMethodsFromType(object instance)
        {
            return AutoMethodsFromType(instance.GetType(), instance);
        }

        public static RegisteredEventType[] AutoMethodsFromType(Type type, object instance)
        {
            // Hotload Issue, sometimes the method breaks when hotloading... :(
            if (!m_Libraries.TryGetValue(type, out var items) || items.Any(e => e.Method == null))
            {
                m_Libraries.Remove(type);
                AssignMethodToCache(type);
                items = m_Libraries[type];
            }

            var library = new RegisteredEventType[items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                var callback = items[i].Method.CreateDelegate(items[i].Delegate, instance);
                library[i] = new RegisteredEventType(items[i].Event, callback);
            }

            return library;
        }

        private static Type CreateDelegateFromMethod(MethodDescription method, Type evt)
        {
            Type delegateType = null;

            const bool isRef = false; // method.Parameters[0].ParameterType.IsByRef;
            if (method.ReturnType == typeof(Task))
            {
                delegateType = isRef ? typeof(RefAsyncStructCallback<>) : typeof(AsyncStructCallback<>);
            }

            delegateType ??= isRef ? typeof(RefStructCallback<>) : typeof(StructCallback<>);
            return TypeLibrary.GetType(delegateType).MakeGenericType(new[] { evt });
        }

        private static void AssignMethodToCache(Type type)
        {
            var methods = TypeLibrary.GetType(type).Methods.Where(e => !e.IsStatic && e.GetCustomAttribute<ListenAttribute>()?.Global == false).ToArray();
            var cache = new (MethodDescription methodInfo, Type Delegate, Type Event)[methods.Length];

            for (var i = 0; i < methods.Length; i++)
            {
                var methodInfo = methods[i];
                var parameters = methodInfo.Parameters;

                if (parameters.Length != 1)
                    throw new Exception("Invalid Auto Parameters");

                var parameterType = TypeLibrary.GetGenericArguments(parameters[0].ParameterType)[0];
                var delegateType = CreateDelegateFromMethod(methodInfo, parameterType);
                cache[i] = (methodInfo, delegateType, parameterType);
            }

            m_Libraries.Add(type, cache);
        }
#endif
    }
}