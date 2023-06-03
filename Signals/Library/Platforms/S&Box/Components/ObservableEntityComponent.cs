﻿#if SANDBOX
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Signals
{
    public abstract class ObservableEntityComponent : EntityComponent
    {
        protected virtual IDispatcher Events
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Entity as IObservableEntity)?.Events ?? throw new InvalidCastException($"Entity is not observable - {Entity.GetType().FullName}");
        }

        private RegisteredEventType[] m_Events;

        protected override void OnActivate()
        {
            var recoding = Events.Record();
            using (recoding)
            {
                OnAutoRegister();
            }

            m_Events = recoding.Events.ToArray();
        }

        protected virtual void OnAutoRegister() { }

        protected override void OnDeactivate()
        {
            if (m_Events is not { Length: > 0 })
                return;

            foreach (var item in m_Events)
            {
                Events.Unregister(item);
            }
        }

        // Register

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Register<T>(StructCallback<T> evt) where T : struct, ISignal => Events.Register(evt);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Register<T>(Action evt) where T : struct, ISignal => Events.Register<T>(evt);

        // Run

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Run<T>(T data = default, Propagation propagation = Propagation.None) where T : struct, ISignal
        {
            if (Entity == null)
                return;

            Events.Run(data, propagation);
        }

        // Unregister

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Unregister<T>(StructCallback<T> evt) where T : struct, ISignal => Events.Unregister(evt);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Unregister<T>(Action evt) where T : struct, ISignal => Events.Unregister<T>(evt);
    }

    public abstract class ObservableEntityComponent<T> : ObservableEntityComponent where T : class, IObservableEntity
    {
        public new T Entity => base.Entity as T;
        protected Entity UnderlyingEntity => base.Entity;

        public override bool CanAddToEntity(Entity entity) => entity is T;
    }
}

#endif