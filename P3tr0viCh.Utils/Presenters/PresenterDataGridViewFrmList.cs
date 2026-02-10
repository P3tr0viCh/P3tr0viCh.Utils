using P3tr0viCh.Utils.Comparers;

namespace P3tr0viCh.Utils.Presenters
{
    internal class PresenterDataGridViewFrmList<T> : PresenterDataGridView<T> where T : IBaseId, new()
    {
        private readonly PresenterFrmListBase<T> presenterFrmList;

        public PresenterDataGridViewFrmList(PresenterFrmListBase<T> presenterFrmList) :
            base(presenterFrmList.FrmList.DataGridView)
        {
            this.presenterFrmList = presenterFrmList;
        }

        public override int Compare(T x, T y, string dataPropertyName, ComparerSortOrder sortOrder)
        {
            return presenterFrmList.Compare(x, y, dataPropertyName, sortOrder);
        }
    }
}