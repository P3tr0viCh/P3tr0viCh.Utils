using P3tr0viCh.Utils.Extensions;
using System;

namespace P3tr0viCh.Utils
{
    public static class Str
    {
        public const string Eol = "\n";
        public const string Space = " ";

        public static string Random(int length, Random rnd = null, string seed = null)
        {
            if (seed.IsEmpty())
            {
                seed = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            }

            if (rnd == null) rnd = new Random();

            var result = string.Empty;

            for (var i = 0; i < length; i++)
            {
                result += seed[rnd.Next(seed.Length)];
            }

            return result;
        }
    }
}