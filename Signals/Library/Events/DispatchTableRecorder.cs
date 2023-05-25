using System;
using System.Collections.Generic;

namespace Woosh.Signals
{
    public readonly struct DispatchTableRecorder : IDisposable
    {
        private readonly IDispatchTable m_Table;
        private readonly HashSet<RegisteredEventType> m_Events;

        public DispatchTableRecorder(IDispatchTable table)
        {
            m_Table = table;
            m_Events = new HashSet<RegisteredEventType>();

            m_Table.Registered += TableOnRegistered;
        }

        private void TableOnRegistered(RegisteredEventType obj) => m_Events.Add(obj);

        public IEnumerable<RegisteredEventType> Events => m_Events;

        public void Dispose()
        {
            m_Table.Registered -= TableOnRegistered;
        }
    }
}