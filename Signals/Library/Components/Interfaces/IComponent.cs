using System.Collections.Generic;

namespace Woosh.Signals
{
    /// <summary>
    /// Tells <see cref="Components{T}"/> that this component should only be attached once. If it is attached again, it will
    /// not be added to the components bucket and throw an exception.
    /// </summary>
    public interface ISingletonComponent { }

    public interface IComponent<T> where T : class
    {
        T Attached { get; set; }
        LinkedListNode<IComponent<T>> Node { get; set; }

        bool Attachable(T item) { return true; }
        virtual void OnAttached() { }
        virtual void OnDetached() { }
    }
}
