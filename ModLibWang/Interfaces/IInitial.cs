namespace ModLibWang.Interfaces
{
    public interface IInitial
    {
        Ref Context { get; }
        object Value { get; }
        void Reset();
        bool Changed();
    }
}
