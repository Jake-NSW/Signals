namespace Woosh.Signals
{
    public interface IAspect<T> where T : class
    {
        void ExportTo(Components<T> system);
    }
}