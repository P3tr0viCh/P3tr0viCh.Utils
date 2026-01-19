using P3tr0viCh.Utils.Extensions;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace P3tr0viCh.Utils
{
    public static partial class Misc
    {
        public const string P3tr0viCh = "P3tr0viCh";

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

        public static DateTime DateTimeParse(string str, DateTime def = default)
        {
            return DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal,
                out DateTime result) ? result : def;
        }

        public static DateTime DateTimeParseExact(string str, string format, DateTime def = default)
        {
            return DateTime.TryParseExact(str, format, null, DateTimeStyles.AssumeLocal,
                out DateTime result) ? result : def;
        }

        public static double DoubleParse(string str, double def = 0.0)
        {
            return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture,
                out double result) ? result : def;
        }

        public static double DoubleParseInvariant(string str, double def = 0.0)
        {
            return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                out double result) ? result : def;
        }

        public static bool DoubleCheck(string str)
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

            var str = string.Empty;

            try
            {
                var assembly = Assembly.GetEntryAssembly();

                var baseName = $"{assembly.GetName().Name}.{ResourcesName}";

                var res = new ResourceManager(baseName, assembly);

                str = res.GetString(ResourceKey);
            }
            catch (Exception)
            {
            }

            return str.IsEmpty() ? ResourceKey : str;
        }

        public static Color HexToColor(string hex)
        {
            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }

            int r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            int g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            int b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromArgb(r, g, b);
        }
    }
}