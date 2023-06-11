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
    public class ListenAttribute : Attribute
    {
        public bool Global { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
#if UNITY
    [RequireAttributeUsages, Preserve]
#endif
    [Obsolete]
    public sealed class AutoAttribute : ListenAttribute { }
}
