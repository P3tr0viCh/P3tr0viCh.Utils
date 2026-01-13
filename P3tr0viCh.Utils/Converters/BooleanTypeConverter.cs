using P3tr0viCh.Utils.Properties;
using System;
using System.ComponentModel;
using System.Globalization;

namespace P3tr0viCh.Utils.Converters
{
    public class BooleanTypeConverter : BooleanConverter
    {
        private static volatile StandardValuesCollection values;

        public string TrueValue { get; set; } = "true";
        public string FalseValue { get; set; } = "false";

        public BooleanTypeConverter()
        {
        }

        public BooleanTypeConverter(string trueValue, string falseValue)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (values == null)
            {
                values = new StandardValuesCollection(new object[2] { true, false });
            }

            return values;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            return (bool)value ? TrueValue : FalseValue;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return ((string)value).Equals(TrueValue);
        }
    }

    public class BooleanTypeOnOffConverter : BooleanTypeConverter
    {
        public BooleanTypeOnOffConverter() : base(
            Misc.GetResourceString("OnOffConverter.On", Consts.ResourceName),
            Misc.GetResourceString("OnOffConverter.Off", Consts.ResourceName))
        { }
    }
}