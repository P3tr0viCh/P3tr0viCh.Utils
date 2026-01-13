using System.ComponentModel;

namespace P3tr0viCh.Utils.Attributes
{
    public sealed class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(string resourceKey, string resourcesName) :
            base(Misc.GetResourceString(resourceKey, resourcesName))
        {
        }
    }

    public sealed class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(string resourceKey, string resourcesName) :
            base(Misc.GetResourceString(resourceKey, resourcesName))
        {
        }
    }

    public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(string resourceKey, string resourcesName) :
            base(Misc.GetResourceString(resourceKey, resourcesName))
        {
        }
    }
}