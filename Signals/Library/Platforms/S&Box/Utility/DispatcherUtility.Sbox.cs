#if SANDBOX
using System;
using Sandbox;

namespace Woosh.Signals;

public partial class DispatcherUtility
{
    public static void SendEvent<T>(this IClient client) where T : struct, ISignal
    {
        Dispatcher.FindForEntity((Entity)client).Run<T>();
    }

    public static void SendEvent<T>(this IClient client, T signal) where T : struct, ISignal
    {
       Dispatcher.FindForEntity((Entity)client).Run(signal);
    }

    public static void RegisterForEvent<T>(this IClient client, Action callback) where T : struct, ISignal
    {
        // Find Dispatcher Component
        Dispatcher.FindForEntity((Entity)client).Register<T>(callback);
    }

    public static void RegisterForEvent<T>(this IClient client, StructCallback<T> callback) where T : struct, ISignal
    {
        // Find Dispatcher Component
      Dispatcher.FindForEntity((Entity)client).Register(callback);
    }
}
#endif
