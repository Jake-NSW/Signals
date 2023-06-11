using System;
#if UNITY
using UnityEngine.Scripting;
#endif

namespace Woosh.Signals
{
    [AttributeUsage(AttributeTargets.Method)]
#if UNITY
    [RequireAttributeUsages, Preserve]
#endif
    public class ListenAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
#if UNITY
    [RequireAttributeUsages, Preserve]
#endif
    public sealed class CallbackAttribute : Attribute
    {
        public Type Type { get; }

        public CallbackAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
#if UNITY
    [RequireAttributeUsages, Preserve]
#endif
    [Obsolete]
    public sealed class AutoAttribute : ListenAttribute { }
}
