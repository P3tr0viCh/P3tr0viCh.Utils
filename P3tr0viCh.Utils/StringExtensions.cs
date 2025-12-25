using System;

namespace P3tr0viCh.Utils
{
    public static class StringExtensions
    {
        private static int IndexOfNth(this string s, string value, int place, int startIndex = 0)
        {
            if (s.IsEmpty()) return -1;

            if (string.IsNullOrEmpty(value)) return -1;

            if (place == 0) return s.IndexOf(value, startIndex);

            return s.IndexOfNth(value, --place, s.IndexOf(value, startIndex) + 1);
        }

        public static int IndexOfNth(this string s, string value, int place)
        {
            return IndexOfNth(s, value, place, 0);
        }

        public static string JoinExcludeEmpty(this string s, string separator, string value)
        {
            if (s.IsEmpty() && value.IsEmpty()) return string.Empty;

            if (s.IsEmpty()) return value;

            if (value.IsEmpty()) return s;

            return s + separator + value;
        }

        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool IsInt(this string s)
        {
            return int.TryParse(s, out _);
        }

        public static bool IsDouble(this string s)
        {
            return double.TryParse(s, out _);
        }

        public static string ReplaceEol(this string s)
        {
            return s?.Replace("\r\n", Str.Space).Replace("\n", Str.Space).Replace("\r", Str.Space);
        }

        public static string TrimText(this string s)
        {
            if (s == null) return null;

            if (s.IsEmpty()) return null;

            return s.Trim();
        }

        public static string SingleLine(this string s)
        {
            char[] charsToTrim = { ' ', '\n', '\r', '\t' };

            var parts = s.Split(charsToTrim, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(Str.Space, parts);
        }
    }
}