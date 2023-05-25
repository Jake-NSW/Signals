using System;

namespace Woosh.Signals
{
    public readonly struct RegisteredEventType
    {
        public RegisteredEventType(Type @event, Delegate @delegate)
        {
            Event = @event;
            Delegate = @delegate;
        }

        public override int GetHashCode()
        {
            return Event.GetHashCode() ^ Delegate.GetHashCode();
        }

        public Type Event { get; }
        public Delegate Delegate { get; }
    }
}