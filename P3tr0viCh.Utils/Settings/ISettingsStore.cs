namespace P3tr0viCh.Utils.Settings
{
    public interface ISettingsStore
    {
        object SelectedObject { get; }

        bool Load();

        bool Save();
    }
}