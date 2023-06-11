using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Woosh.Signals
{
    internal sealed class WeakList<T> : IEnumerable<T> where T : class
    {
        public WeakList(int initialCapacity)
        {
            m_Bucket = new List<WeakReference>(initialCapacity);
            for (var i = 0; i < initialCapacity; i++)
            {
                m_Bucket[i] = new WeakReference(null);
            }
        }

        private readonly List<WeakReference> m_Bucket;
        private readonly object m_Lock = new object();

        public void Add(T item)
        {
            if (item == null)
                return;

            lock (m_Lock)
            {
                var i = 0;
                while (i < m_Bucket.Count)
                {
                    // Already in bucket
                    if (this[i] == item)
                    {
                        return;
                    }

                    if (this[i] == null)
                    {
                        // Replace WeakReference with new item
                        this[i] = item;
                        return;
                    }

                    i++;
                }

                m_Bucket.Add(new WeakReference(item));
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Bucket.Count)
                    return null;

                return m_Bucket[index].Target as T;
            }
            set
            {
                if (index < 0 || index >= m_Bucket.Count)
                {
                    return;
                }

                m_Bucket[index].Target = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_Bucket.Where(e => e.IsAlive).Select(e => (T)e.Target).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
