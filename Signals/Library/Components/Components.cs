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
            m_RegisteredSingletons = new HashSet<Type>();
        }

        ~Components()
        {
            Clear();
        }

        private readonly HashSet<Type> m_RegisteredSingletons;
        private readonly LinkedList<IComponent<T>> m_Storage;

        public TComp Add<TComp>(TComp item) where TComp : class, IComponent<T>
        {
            if (item == null)
            {
                return null;
            }

            if (item is ISingletonComponent && m_RegisteredSingletons.Contains(item.GetType()))
            {
                throw new InvalidOperationException("Singleton component was already attached");
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
            m_RegisteredSingletons.Add(item.GetType());

            return item;
        }

        public IEnumerable<TComp> All<TComp>() where TComp : class, IComponent<T>
        {
            return m_Storage.OfType<TComp>();
        }

        public bool Contains(IComponent<T> item)
        {
            return item.Node?.List != null;
        }

        public void Remove(IComponent<T> item)
        {
            Detach(item);
            m_Storage.Remove(item.Node);
            m_RegisteredSingletons.Remove(item.GetType());
            item.Node = null;
        }

        public void Remove<TComp>() where TComp : class, IComponent<T>
        {
            Remove(Get<TComp>());
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

        public TComp Get<TComp>() where TComp : class, IComponent<T>
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TComp Create<TComp>() where TComp : class, IComponent<T>, new()
        {
            return Add(new TComp());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TComp GetOrCreate<TComp>() where TComp : class, IComponent<T>, new()
        {
            return Get<TComp>() ?? Create<TComp>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Import<TAspect>() where TAspect : struct, IAspect<T>
        {
            new TAspect().Import(this);
        }
    }
}
