using System;
using System.Reflection;
using System.Resources;

namespace P3tr0viCh.Utils
{
    public class LocalizedAttribute
    {
        private const string DefaultResourcesName = "P3tr0vich.Localized";

        private static string GetResourceString(string ResourceKey, string ResourcesName)
        {
            var assembly = Assembly.GetEntryAssembly();

            var baseName = $"{assembly.GetName().Name}.{(ResourcesName.IsEmpty() ? DefaultResourcesName : ResourcesName)}";

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

        public sealed class CategoryAttribute : System.ComponentModel.CategoryAttribute
        {
            public CategoryAttribute(string resourceKey, string resourcesName) : base(GetResourceString(resourceKey, resourcesName))
            {
            }
        }

        public sealed class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
        {
            public DisplayNameAttribute(string resourceKey, string resourcesName) : base(GetResourceString(resourceKey, resourcesName))
            {
            }
        }

        public sealed class DescriptionAttribute : System.ComponentModel.DescriptionAttribute
        {
            public DescriptionAttribute(string resourceKey, string resourcesName) : base(GetResourceString(resourceKey, resourcesName))
            {
            }
        }
    }
}