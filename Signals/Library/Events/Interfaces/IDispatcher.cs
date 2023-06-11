namespace Woosh.Signals
{
    /// <summary>
    /// A combination of <see cref="IDispatchExecutor"/> and <see cref="IDispatchTable"/>.
    /// </summary>
    public interface IDispatcher : IDispatchExecutor, IDispatchTable { }
}
