using System;

namespace P3tr0viCh.Utils
{
    public static class Str
    {
        public readonly static string Eol = "\n";
        public readonly static string Space = " ";

        public static string Random(int length, Random rnd = null, string seed = null)
        {
            if (string.IsNullOrEmpty(seed))
            {
                seed = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            }

            if (rnd == null) rnd = new Random();

            var result = "";

            for (int i = 0; i < length; i++)
            {
                result += seed[rnd.Next(seed.Length)];
            }

            return result;
        }

        private static int IndexOfNth(this string s, string value, int place, int startIndex = 0)
        {
            if (string.IsNullOrEmpty(s)) return -1;
            if (string.IsNullOrEmpty(value)) return -1;

            if (place == 0)
                return s.IndexOf(value, startIndex);

            return s.IndexOfNth(value, --place, s.IndexOf(value, startIndex) + 1);
        }

        public static int IndexOfNth(this string s, string value, int place)
        {
            return IndexOfNth(s, value, place, 0);
        }

        public static string JoinExcludeEmpty(this string s, string separator, string value)
        {
            if (string.IsNullOrEmpty(s) && string.IsNullOrEmpty(value)) return s;

            if (string.IsNullOrEmpty(s)) return value;

            if (string.IsNullOrEmpty(value)) return s;

            return s + separator + value;
        }
        
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }
    }
}