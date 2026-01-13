using System;
using System.ComponentModel;
using System.Globalization;

namespace P3tr0viCh.Utils.Converters
{
    public class EnumDescriptionConverter : EnumConverter
    {
        public EnumDescriptionConverter(Type type) : base(type)
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            return value.Description();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            foreach (var field in EnumType.GetFields())
            {
                if (field.Description() == value.ToString())
                {
                    return Enum.Parse(EnumType, field.Name);
                }
            }

            return Enum.Parse(EnumType, (string)value);
        }
    }
}