using P3tr0viCh.Utils.Delegates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P3tr0viCh.Utils.Interfaces
{
    public interface IPresenterFrmList
    {
        IFrmList FrmList { get; }

        event FrmListChangedEventHandler FrmListChanged;

        bool Changed { get; }

        Task ListItemAddNewAsync();

        Task ListItemChangeSelectedAsync();

        Task ListItemDeleteSelectedAsync();

        void ListItemChange(IBaseId value);

        void ListItemChange(IEnumerable<IBaseId> list);

        void ListItemDelete(IBaseId value);

        void ListItemDelete(IEnumerable<IBaseId> list);
    }
}