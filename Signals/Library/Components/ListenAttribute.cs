using System;
#if UNITY
using UnityEngine.Scripting;
#endif

namespace Woosh.Signals
{
    /// <summary>
    /// The Listen attribute represents a method that can be invoked via a dispatcher. Sometimes this is automatic by the inheriting
    /// type, or done via <see cref="ObservableUtility.AutoRegisterEvents"/>. Some methods can be global, meaning it will only be able
    /// to be invoked by the global dispatcher. Static methods are always considered to be registered to the global dispatcher.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
#if UNITY
    [RequireAttributeUsages, Preserve]
#endif
    public sealed class ListenAttribute : Attribute
    {
        /// <summary>
        /// Marks either or not this function should be invoked on the global dispatcher or the local one.
        /// </summary>
        public bool Global { get; set; }
    }
}
