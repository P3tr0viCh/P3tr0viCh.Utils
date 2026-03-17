using System.Collections.Generic;

namespace P3tr0viCh.Utils.Interfaces
{
    public interface IUpdateDataList
    {
        void ListItemsChange(IEnumerable<IBaseId> list);

        void ListItemsDelete(IEnumerable<IBaseId> list);
    }
}