using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Woosh.Signals
{
    public static class ObservableUtility
    {
        static ObservableUtility()
        {
            m_Libraries = new Dictionary<Type, (MethodInfo Method, Type Event)[]>();
        }

        private readonly static Dictionary<Type, (MethodInfo Method, Type Event)[]> m_Libraries;

        public static Span<(Type Event, Delegate Delegate)> AutoMethodsFromType(Type type, object instance)
        {
            if (!m_Libraries.TryGetValue(type, out var items))
                return AssignMethodToCache(type, instance);

            Span<(Type Event, Delegate Delegate)> library = new (Type Event, Delegate Delegate)[items.Length];

            for (var i = 0; i < items.Length; i++)
            {
                var callback = items[i].Method.CreateDelegate(typeof(Action<>).MakeGenericType(items[i].Event), instance);
                library[i] = (items[i].Event, callback);
            }

            return library;
        }

        public static Span<(Type Event, Delegate Delegate)> AssignMethodToCache(Type type, object instance)
        {
            var methods = type.GetMethods(BindingFlags.Instance).Where(m => m.GetCustomAttribute<AutoAttribute>() != null).ToArray();

            Span<(Type Event, Delegate Delegate)> library = new (Type Event, Delegate Delegate)[methods.Length];
            var cache = new (MethodInfo methodInfo, Type Event)[methods.Length];

            for (var i = 0; i < methods.Length; i++)
            {
                var methodInfo = methods[i];
                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 1)
                {
                    throw new MethodAccessException("Invalid number of parameters");
                }

                var parameterType = parameters[0].ParameterType;
                var callback = methodInfo.CreateDelegate(typeof(Action<>).MakeGenericType(parameterType), instance);

                cache[i] = (methodInfo, parameterType);
                library[i] = (parameterType, callback);
            }

            m_Libraries.Add(type, cache);
            return library;
        }
    }
}
