namespace P3tr0viCh.Utils.Comparers
{
    public class EmptyStringComparer: ISortOrderComparer<string>
    {
        public static readonly EmptyStringComparer Default = new EmptyStringComparer();

        public int Compare(string x, string y, ComparerSortOrder sortOrder)
        {
            if (string.IsNullOrEmpty(x))
            {
                return string.IsNullOrEmpty(y) ? 0 : 1;
            }

            if (string.IsNullOrEmpty(y))
            {
                return -1;
            }

            var result = x.CompareTo(y);

            return sortOrder == ComparerSortOrder.Ascending ? result : -result;
        }
    }
}