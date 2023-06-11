﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Woosh.Signals
{
    internal sealed class GlobalDispatcher : IDispatchExecutor, IDisposable
    {
        private delegate void MethodDelegate<T>(Event<T> data) where T : struct, ISignal;

        private readonly Dictionary<Type, WeakList<object>> m_References = new Dictionary<Type, WeakList<object>>();

        public bool Run<T>(Event<T> data, Propagation propagation) where T : struct, ISignal
        {
            if (!m_Library.TryGetValue(typeof(T), out var methods))
                return false;

            foreach (var (method, type) in methods)
            {
                if (method.IsStatic)
                {
                    ((MethodDelegate<T>)method.CreateDelegate(typeof(MethodDelegate<T>))).Invoke(data);
                }
                else
                {
                    if (!m_References.TryGetValue(type, out var references))
                        continue;

                    foreach (var target in references)
                        ((MethodDelegate<T>)method.CreateDelegate(typeof(MethodDelegate<T>), target)).Invoke(data);
                }
            }

            return true;
        }

        public Task RunAsync<T>(Event<T> data, Propagation propagation) where T : struct, ISignal
        {
            throw new InvalidOperationException("Not Supported");
        }

        public void Register(object item)
        {
            var type = item.GetType();
            if (!m_References.TryGetValue(type, out var collection))
            {
                m_References.Add(type, collection = new WeakList<object>(1));
            }

            AddToLibrary(item.GetType());
            collection.Add(item);
        }

        private readonly static Dictionary<Type, List<(MethodInfo, Type)>> m_Library = new Dictionary<Type, List<(MethodInfo, Type)>>();
        private readonly static HashSet<Type> m_Registered = new HashSet<Type>();

        private static void AddToLibrary(Type type)
        {
            // Don't re-register the same things
            if (m_Registered.Contains(type))
                return;

            var methods = type.GetMethods().Where(e => e.IsDefined(typeof(ListenAttribute)));
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<ListenAttribute>();

                if (!method.IsStatic && attribute.Global == false)
                    continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    throw new InvalidOperationException($"Method {method.Name} on type {type.Name} is declared with the wrong parameters");

                var parameter = parameters[0].ParameterType.GenericTypeArguments[0];
                if (!m_Library.TryGetValue(parameter, out var collection))
                {
                    m_Library.Add(parameter, collection = new List<(MethodInfo, Type)>(1));
                }

                collection.Add((method, type));
            }

            m_Registered.Add(type);
        }

        public void Dispose()
        {
            m_References.Clear();
        }
    }
}
