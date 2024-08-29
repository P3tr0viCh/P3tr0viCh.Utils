namespace P3tr0viCh.Utils
{
    public class LocalizedAttribute
    {
        public sealed class CategoryAttribute : System.ComponentModel.CategoryAttribute
        {
            public CategoryAttribute(string resourceKey, string resourcesName) : base(Misc.GetResourceString(resourceKey, resourcesName))
            {
            }
        }

        public sealed class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
        {
            public DisplayNameAttribute(string resourceKey, string resourcesName) : base(Misc.GetResourceString(resourceKey, resourcesName))
            {
            }
        }

        public sealed class DescriptionAttribute : System.ComponentModel.DescriptionAttribute
        {
            public DescriptionAttribute(string resourceKey, string resourcesName) : base(Misc.GetResourceString(resourceKey, resourcesName))
            {
            }
        }
    }
}