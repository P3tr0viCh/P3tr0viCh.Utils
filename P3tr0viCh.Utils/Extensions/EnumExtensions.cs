using System;
using System.Collections.Generic;
using System.Linq;

namespace P3tr0viCh.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue);

        public static T AddFlag<T>(this T value, T flag) where T : struct, Enum
        {
            return (T)(object)((int)(object)value | (int)(object)flag);
        }

        public static T RemoveFlag<T>(this T value, T flag) where T : struct, Enum
        {
            return (T)(object)((int)(object)value & ~(int)(object)flag);
        }

        public static IEnumerable<T> GetValues<T>() where T : struct, Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T All<T>() where T : struct, Enum
        {
            T result = default;

            foreach(var item in GetValues<T>())
            {
                result = result.AddFlag(item);
            }

            return result;
        }
    }
}