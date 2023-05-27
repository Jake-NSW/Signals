using System.Collections.Generic;

namespace Woosh.Signals
{
    public interface IObservable
    {
        public IDispatcher Events { get; }

        IEnumerable<IDispatcher> OnTrickleEvent() { return null; }
        IDispatcher OnBubbleEvent() { return null; }
    }
}
