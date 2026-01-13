using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;

namespace P3tr0viCh.Utils.Converters
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
        private readonly List<string> strings;

        private readonly bool exclusive;

        public StringListConverter(string resourceKey, string resourcesName, bool exclusive)
        {
            var resource = Misc.GetResourceString(resourceKey, resourcesName);

            strings = resource.Split(
                new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            this.exclusive = exclusive;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return exclusive; }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(strings);
        }
    }
}