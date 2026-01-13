using System;
using System.Collections.Generic;

namespace P3tr0viCh.Utils
{
    public class PathComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Equals(Files.PathNormalize(x), Files.PathNormalize(y), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLowerInvariant().GetHashCode();
        }
    }
}