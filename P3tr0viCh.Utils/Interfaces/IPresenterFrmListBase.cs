using System;
using System.Threading.Tasks;

namespace P3tr0viCh.Utils.Interfaces
{
    public interface IPresenterFrmListBase: IFrmUpdateData, IFrmUpdateDataList
    {
        IFrmList FrmList { get; }

        event EventHandler ListChanged;

        bool Changed { get; }

        Task AddNewItemAsync();

        Task SelectedChangeAsync();

        Task SelectedDeleteAsync();
    }
}