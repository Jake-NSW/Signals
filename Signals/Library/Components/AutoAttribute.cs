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
    public sealed class AutoAttribute : Attribute { }
}