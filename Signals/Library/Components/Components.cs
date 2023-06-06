using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Woosh.Signals
{
    public delegate void RefStructInputAction<TInput, in TType>(ref TInput input, TType item);

    public sealed class Components<T> where T : class
    {
        public T Owner { get; }

        public Components(T item)
        {
            Owner = item;
            m_Storage = new LinkedList<IComponent<T>>();
        }

        ~Components()
        {
            Clear();
        }

        private readonly LinkedList<IComponent<T>> m_Storage;

        public TComp Add<TComp>(TComp item) where TComp : class, IComponent<T>
        {
            if (item == null)
            {
                return null;
            }

            if (item.Attached != null)
            {
                throw new InvalidOperationException("Component was already attached to something");
            }

            if (!item.Attachable(Owner))
            {
                return null;
            }

            var node = new LinkedListNode<IComponent<T>>(item);

            item.Node = node;
            item.Attached = Owner;

            m_Storage.AddLast(node);
            item.OnAttached();

            return item;
        }

        public bool Contains(IComponent<T> item)
        {
            return item.Node?.List != null;
        }

        public void Remove(IComponent<T> item)
        {
            Detach(item);
            m_Storage.Remove(item.Node);
            item.Node = null;
        }

        public void Clear()
        {
            foreach (var comp in m_Storage)
            {
                Detach(comp);
            }

            m_Storage.Clear();
        }

        private void Detach(IComponent<T> item)
        {
            item.OnDetached();
            item.Attached = null;
        }

        public int Count => m_Storage.Count;

        public TComp Get<TComp>() where TComp : class, IComponent
        {
            return m_Storage.FirstOrDefault(e => e is TComp) as TComp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<TType>(Action<TType> action)
        {
            foreach (var item in m_Storage.OfType<TType>())
            {
                action?.Invoke(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<TInput, TType>(ref TInput input, RefStructInputAction<TInput, TType> action)
        {
            foreach (var item in m_Storage.OfType<TType>())
            {
                action?.Invoke(ref input, item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<TInput, TType>(TInput input, Action<TInput, TType> action)
        {
            foreach (var item in m_Storage.OfType<TType>())
            {
                action?.Invoke(input, item);
            }
        }

        public TComp Create<TComp>() where TComp : class, IComponent<T>, new()
        {
            return Add(new TComp());
        }

        public TComp GetOrCreate<TComp>() where TComp : class, IComponent<T>, new()
        {
            return Get<TComp>() ?? Create<TComp>();
        }
    }
}
