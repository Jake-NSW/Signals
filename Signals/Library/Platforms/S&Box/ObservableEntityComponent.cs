#if SANDBOX
using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Signals
{
    public abstract class ObservableEntityComponent<T> : ObservableEntityComponent where T : class, IObservableEntity
    {
        public new T Entity => base.Entity as T;
        protected Entity UnderlyingEntity => base.Entity;

        public override bool CanAddToEntity(Entity entity) => entity is T;
    }
    
    public abstract class ObservableEntityComponent : EntityComponent
    {
        protected IDispatcher Events
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Entity as IObservableEntity)?.Events ?? throw new InvalidCastException($"Entity is not observable - {Entity.GetType().FullName}");
        }

        // Register

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Register<T>(StructCallback<T> evt) where T : struct, ISignal => Events.Register(evt);

        // Run

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Run<T>(T data = default) where T : struct, ISignal
        {
            Events.Run(data, this);
        }

        // Unregister

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Unregister<T>(StructCallback<T> evt) where T : struct, ISignal => Events.Unregister(evt);
    }
}

#endif
