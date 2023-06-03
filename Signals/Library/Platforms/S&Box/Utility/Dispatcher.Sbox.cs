#if SANDBOX
using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Signals;

partial class Dispatcher
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDispatcher FindForEntity(IEntity entity)
    {
        return entity switch
        {
            IObservable observable => observable.Events,
            IClient client => client.Components.GetOrCreate<ClientEventDispatcher>().Events,
            _ => null
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDispatcher CreateForClient(IClient client)
    {
        return new Dispatcher(
            attached: client,
            bubble: static o => null,
            trickle: static o =>
            {
                var client = (IClient)o;
                return new[] { (client.Pawn as IObservable)?.Events };
            }
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDispatcher CreateForEntity<T>(T entity) where T : Entity, IObservable
    {
        return new Dispatcher(
            attached: entity,
            bubble: static o =>
            {
                var ent = (Entity)o;

                if (ent.Parent is IObservable observable)
                {
                    return observable.Events;
                }

                if (ent.Client != null && ent.Parent == null)
                {
                    return FindForEntity(ent.Client);
                }

                return null;
            },
            trickle: static o =>
            {
                var ent = (Entity)o;
                var children = ent.Children;

                if (children.Count == 0)
                {
                    return Array.Empty<IDispatcher>();
                }

                var array = new IDispatcher[children.Count];

                for (var i = 0; i < ent.Children.Count; i++)
                {
                    var child = ent.Children[i];
                    if (child is IObservable observable)
                        array[i] = observable.Events;
                }

                return array;
            }
        );
    }
}
#endif
