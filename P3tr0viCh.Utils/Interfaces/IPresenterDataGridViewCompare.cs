using P3tr0viCh.Utils.Comparers;

namespace P3tr0viCh.Utils.Interfaces
{
    public interface IPresenterDataGridViewCompare<T> where T : IBaseId
    {
        int Compare(T x, T y, string dataPropertyName, ComparerSortOrder sortOrder);
    }
}