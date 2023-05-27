#if SANDBOX
using System.Collections.Generic;
using Sandbox;

namespace Woosh.Signals
{
    public interface IObservableEntity : IObservable, IEntity
    {
        IDispatcher IObservable.OnBubbleEvent()
        {
            if (Parent is IObservable observable)
                return observable.Events;
            
            return null;
        }

        IEnumerable<IDispatcher> IObservable.OnTrickleEvent()
        {
            return null;

            /* -- Not supported yet...
            foreach (var ent in Children)
            {
                if (ent is IObservable observable)
                    yield return observable.Events;
            }
            */
        }
    }
}
#endif
