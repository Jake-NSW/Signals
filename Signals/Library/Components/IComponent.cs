using System.Collections.Generic;

namespace Woosh.Signals
{
    public interface IComponent { }

    public interface IComponent<T> : IComponent where T : class
    {
        T Attached { get; set; }
        LinkedListNode<IComponent<T>> Node { get; set; }

        bool Attachable(T item) { return true; }
        virtual void OnAttached() { }
        virtual void OnDetached() { }
    }
}
