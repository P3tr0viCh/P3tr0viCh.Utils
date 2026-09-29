namespace P3tr0viCh.Utils.Storage
{
    public interface IObjectStorage
    {
        IObjectPersistence Object { get; }

        bool Load();

        bool Save();
    }
}