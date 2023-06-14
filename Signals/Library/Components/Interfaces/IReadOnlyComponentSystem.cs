using System.Collections.Generic;

namespace Woosh.Signals
{
    public interface IReadOnlyComponentSystem<T> where T : class
    {
        TComp Get<TComp>() where TComp : class, IComponent<T>;
        bool Has<TComp>() where TComp : class, IComponent<T>;
        IEnumerable<TComp> All<TComp>() where TComp : class, IComponent<T>;
    }
}
