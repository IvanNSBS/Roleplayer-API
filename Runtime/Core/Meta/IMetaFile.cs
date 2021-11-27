namespace INUlib.Core.Meta
{
    public interface IMetaFile<T> where T : class
    {
        T Data { get; }
        bool Load();
    }
}