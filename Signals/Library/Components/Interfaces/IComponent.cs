using System.Collections.Generic;

namespace Woosh.Signals
{
#if !SANDBOX
    /// <summary>
    /// Tells <see cref="Components{T}"/> that this component should only be attached once. If it is attached again, it will
    /// not be added to the components bucket and throw an exception.
    /// </summary>
    public interface ISingletonComponent { }
#endif

    /// <summary>
    /// The base interface for all components. Components are attached to <see cref="Components{T}"/> and are used to add
    /// functionality to a class without having to inherit from it. 
    /// </summary>
    public interface IComponent<T> where T : class
    {
        T Attached { get; set; }
        LinkedListNode<IComponent<T>> Node { get; set; }

        bool Attachable(T item) => true;

        void OnAttached() { }
        void OnDetached() { }
    }
}