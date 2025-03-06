using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace P3tr0viCh.Utils
{
    public static partial class Misc
    {
        public static string Description(this object value)
        {
            var field = value is FieldInfo ? value as FieldInfo : value.GetType().GetField(value.ToString());

            if (field.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes
                && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static DateTime DateTimeParse(string str, string format, DateTime def = default)
        {
            return DateTime.TryParseExact(str, format, null, DateTimeStyles.AssumeLocal,
                out DateTime result) ? result : def;
        }

        public static double DoubleParse(string str, double def = 0.0, bool DecimalIsDot = false)
        {
            return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture,
                out double result) ? result : def;
        }

        public static double DoubleParseInvariant(string str, double def = 0.0)
        {
            return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                out double result) ? result : def;
        }

        public static bool DoubleCheck(string str, bool DecimalIsDot = false)
        {
            return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture,
                out _);
        }

        public static bool DoubleCheckInvariant(string str)
        {
            return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                out _);
        }

        public static float FloatParse(string str, float def = 0)
        {
            return float.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture,
                out float result) ? result : def;
        }

        public static float FloatParseInvariant(string str, float def = 0)
        {
            return float.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                out float result) ? result : def;
        }

        public static bool FloatCheck(string str)
        {
            return float.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.CurrentCulture,
                out _);
        }

        public static bool FloatCheckInvariant(string str)
        {
            return float.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                out _);
        }

        public static string GetResourceString(string ResourceKey, string ResourcesName)
        {
            if (ResourcesName.IsEmpty())
            {
                return ResourceKey;
            }

            var assembly = Assembly.GetEntryAssembly();

            var baseName = $"{assembly.GetName().Name}.{ResourcesName}";

            var res = new ResourceManager(baseName, assembly);

            var str = string.Empty;

            try
            {
                str = res.GetString(ResourceKey);
            }
            catch (Exception)
            {
            }

            return str.IsEmpty() ? ResourceKey : str;
        }
    }
}