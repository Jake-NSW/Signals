namespace Woosh.Signals
{
    public interface IAspect<T> where T : class
    {
        void Import(Components<T> system);
    }
}
