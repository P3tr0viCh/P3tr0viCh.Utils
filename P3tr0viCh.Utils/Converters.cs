using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;

namespace P3tr0viCh.Utils
{
    public static class Converters
    {
        public class ComPortStringConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var ports = SerialPort.GetPortNames();

                return new StandardValuesCollection(ports);
            }
        }

        public class StringListConverter : StringConverter
        {
            private readonly List<string> drivers;

            private readonly bool exclusive;

            public StringListConverter(string resourceKey, string resourcesName, bool exclusive)
            {
                var driverResource = Misc.GetResourceString(resourceKey, resourcesName);

                drivers = driverResource.Split(
                    new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

                this.exclusive = exclusive;
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return exclusive; }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(drivers);
            }
        }

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
                Misc.GetResourceString("OnOffConverter.On", "Properties.Resources.Utils"),
                Misc.GetResourceString("OnOffConverter.Off", "Properties.Resources.Utils"))
            { }
        }
    }
}