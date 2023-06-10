using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Woosh.Signals
{
    public static class ObservableUtility
    {
        static ObservableUtility()
        {
            m_Libraries = new Dictionary<Type, (MethodInfo Method, Type Event)[]>();
        }

        private readonly static Dictionary<Type, (MethodInfo Method, Type Event)[]> m_Libraries;

#if !SANDBOX
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
            if (!m_Libraries.TryGetValue(type, out var items))
                return AssignMethodToCache(type, instance);

            Span<RegisteredEventType> library = new RegisteredEventType[items.Length];

            for (var i = 0; i < items.Length; i++)
            {
                var method = items[i].Method;
                var delegateType = method.ReturnType == typeof(Task) ? typeof(AsyncStructCallback<>) : typeof(StructCallback<>);
                var callback = method.CreateDelegate(delegateType.MakeGenericType(items[i].Event), instance);
                library[i] = new RegisteredEventType(items[i].Event, callback);
            }

            return library;
        }

        private static Span<RegisteredEventType> AssignMethodToCache(Type type, object instance)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.GetCustomAttribute<ListenAttribute>() != null).ToArray();

            Span<RegisteredEventType> library = new RegisteredEventType[methods.Length];
            var cache = new (MethodInfo methodInfo, Type Event)[methods.Length];

            for (var i = 0; i < methods.Length; i++)
            {
                var methodInfo = methods[i];
                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 1 && !parameters[0].ParameterType.IsGenericParameter)
                {
                    throw new MethodAccessException("Invalid Auto Parameters");
                }

                var parameterType = parameters[0].ParameterType.GetGenericArguments()[0];
                var delegateType = methodInfo.ReturnType == typeof(Task) ? typeof(AsyncStructCallback<>) : typeof(StructCallback<>);
                var callback = methodInfo.CreateDelegate(delegateType.MakeGenericType(parameterType), instance);

                cache[i] = (methodInfo, parameterType);
                library[i] = new RegisteredEventType(parameterType, callback);
            }

            m_Libraries.Add(type, cache);
            return library;
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<RegisteredEventType> AutoRegisterEvents(object instance, IDispatchTable table)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<RegisteredEventType> AutoMethodsFromType(object instance)
        {
            throw new NotImplementedException();
        }
        
        public static Span<(Type Event, Delegate Delegate)> AutoMethodsFromType(Type type, object instance)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
