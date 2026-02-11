using P3tr0viCh.Utils.Interfaces;
using System.Collections;

namespace P3tr0viCh.Utils.Comparers
{
    public class SortOrderComparer : ISortOrderComparer<object>
    {
        public static readonly SortOrderComparer Default = new SortOrderComparer();

        public int Compare(object x, object y, ComparerSortOrder sortOrder)
        {
            return Comparer.Default.Compare(x, y) * (int)sortOrder;
        }
    }
}