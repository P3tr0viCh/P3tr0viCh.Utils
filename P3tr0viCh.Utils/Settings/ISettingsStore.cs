namespace P3tr0viCh.Utils.Settings
{
    public interface ISettingsStore
    {
        bool Load();

        bool Save();
    }
}