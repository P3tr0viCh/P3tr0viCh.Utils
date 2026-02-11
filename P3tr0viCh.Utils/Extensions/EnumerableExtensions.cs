using System.Collections.Generic;
using System.Linq;

namespace P3tr0viCh.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            return !list.Any();
        }

        public static bool ContainsId<T>(this IEnumerable<T> list, long id) where T : IBaseId
        {
            foreach (var item in list)
            {
                if (item.Id == id) return true;
            }
            return false;
        }

        public static IEnumerable<T> WhereId<T>(this IEnumerable<T> list, long id) where T : IBaseId
        {
            return list.Where(item => item.Id == id);
        }
    }
}