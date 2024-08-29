using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace P3tr0viCh.Utils
{
    public static class Misc
    {
        public class AssemblyDecorator
        {
            public AssemblyDecorator()
            {
                Assembly = Assembly.LoadFrom(Process.GetCurrentProcess().MainModule.FileName);
            }

            public AssemblyDecorator(Assembly assembly)
            {
                Assembly = assembly;
            }

            private Assembly assembly;

            public Assembly Assembly
            {
                get
                {
                    return assembly;
                }
                set
                {
                    assembly = value;

                    Version = assembly.GetName().Version;

                    var assemblyConfiguration = (AssemblyConfigurationAttribute)assembly.GetCustomAttribute(typeof(AssemblyConfigurationAttribute));
                    IsDebug = "Debug".Equals(assemblyConfiguration.Configuration);

                    var assemblyProduct = (AssemblyProductAttribute)assembly.GetCustomAttribute(typeof(AssemblyProductAttribute));
                    Product = assemblyProduct.Product;

                    var assemblyCopyright = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
                    Copyright = assemblyCopyright.Copyright;
                }
            }

            public bool IsDebug { get; private set; }

            public Version Version { get; private set; }

            public string Product { get; private set; }

            public string Copyright { get; private set; }

            public string VersionString(bool full = true, bool withDebug = true)
            {
                var versionString = Version.ToString(full ? 4 : 2);

                if (withDebug && IsDebug)
                {
                    versionString += " (debug build)";
                }

                return versionString;
            }
        }

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

        public static string GetFileTitle(FileVersionInfo info)
        {
            return info.FileDescription;
        }

        public static string GetFileTitle(string filePath)
        {
            return FileVersionInfo.GetVersionInfo(filePath).FileDescription;
        }

        public static Version GetFileVersion(FileVersionInfo info)
        {
            return new Version(info.FileVersion);
        }

        public static Version GetFileVersion(string filePath)
        {
            return GetFileVersion(FileVersionInfo.GetVersionInfo(filePath));
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