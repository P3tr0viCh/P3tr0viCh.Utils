namespace P3tr0viCh.Utils.Comparers
{
    internal interface ISortOrderComparer<T>
    {
        int Compare(T x, T y, ComparerSortOrder sortOrder);
    }
}