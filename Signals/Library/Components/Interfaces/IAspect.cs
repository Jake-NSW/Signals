namespace Woosh.Signals
{
    public interface IAspect<T> where T : class
    {
        void ImportFrom(Components<T> system) { }
        void ExportTo(Components<T> system);
    }
}
