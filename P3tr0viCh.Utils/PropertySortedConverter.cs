using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace P3tr0viCh.Utils
{
    public class PropertySortedConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return false;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            return string.Empty;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var properties = TypeDescriptor.GetProperties(value, attributes);

            var orderedProperties = new List<PropertyOrderPair>();

            foreach (PropertyDescriptor property in properties)
            {
                var attribute = (PropertyOrderAttribute)property.Attributes[typeof(PropertyOrderAttribute)];

                orderedProperties.Add(new PropertyOrderPair(property.Name,
                    attribute != null ? attribute.Value : 0));
            }

            orderedProperties.Sort();

            var propertyNames = new List<string>();

            foreach (var pop in orderedProperties)
            {
                propertyNames.Add(pop.Name);
            }

            return properties.Sort(propertyNames.ToArray());
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        public int Value { get; }

        public PropertyOrderAttribute(int value) => Value = value;
    }

    internal class PropertyOrderPair : IComparable
    {
        public string Name { get; }
        public int Order { get; }

        public PropertyOrderPair(string name, int order)
        {
            Order = order;
            Name = name;
        }

        public int CompareTo(object obj)
        {
            var otherOrder = ((PropertyOrderPair)obj).Order;

            if (otherOrder == Order)
            {
                var otherName = ((PropertyOrderPair)obj).Name;

                return string.Compare(Name, otherName);
            }

            if (otherOrder > Order)
            {
                return -1;
            }

            return 1;
        }
    }
}