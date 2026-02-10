using System.Collections.Generic;

namespace P3tr0viCh.Utils.Interfaces
{
    public interface IFrmUpdateDataList
    {
        void ListItemsChange(IEnumerable<IBaseId> list);

        void ListItemsDelete(IEnumerable<IBaseId> list);
    }
}