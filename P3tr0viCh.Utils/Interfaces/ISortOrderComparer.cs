using P3tr0viCh.Utils.Comparers;

namespace P3tr0viCh.Utils.Interfaces
{
    internal interface ISortOrderComparer<T>
    {
        int Compare(T x, T y, ComparerSortOrder sortOrder);
    }
}