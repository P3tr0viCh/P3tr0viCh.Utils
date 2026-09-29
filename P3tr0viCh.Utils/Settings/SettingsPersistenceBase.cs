using P3tr0viCh.Utils.Storage;

namespace P3tr0viCh.Utils.Settings
{
    public class SettingsPersistenceBase : IObjectPersistence
    {
        public SettingsPersistenceBase()
        {
            Check();
        }

        public virtual void Check()
        {
            if (this is IFormStates formStates && formStates.FormStates == null)
            {
                formStates.FormStates = new FormStates();
            }

            if (this is IColumnStates columnStates && columnStates.ColumnStates == null)
            {
                columnStates.ColumnStates = new ColumnStates();
            }
        }
    }
}