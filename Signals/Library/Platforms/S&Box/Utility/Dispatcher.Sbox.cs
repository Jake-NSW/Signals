﻿#if SANDBOX
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

    // Client

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDispatcher CreateForClient(IClient client)
    {
        return new Dispatcher(
            attached: client,
            bubble: static o => null,
            trickle: OnTrickleClient
        );
    }

    private static IDispatcher[] OnTrickleClient(object o)
    {
        var client = (IClient)o;
        return new[] { (client.Pawn as IObservable)?.Events };
    }

    // Entity

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDispatcher CreateForEntity<T>(T entity) where T : Entity, IObservable
    {
        return new Dispatcher(
            attached: entity,
            bubble: OnBubbleEntity,
            trickle: OnTrickleEntity
        );
    }

    private static IDispatcher OnBubbleEntity(object o)
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
    }

    private static IDispatcher[] OnTrickleEntity(object o)
    {
        var ent = (Entity)o;

        var children = ent.Children;

        if (children.Count == 0)
        {
            return Array.Empty<IDispatcher>();
        }

        var array = new IDispatcher[children.Count];

        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child is IObservable observable)
            {
                array[i] = observable.Events;
            }
        }

        return array;
    }
}
#endif
