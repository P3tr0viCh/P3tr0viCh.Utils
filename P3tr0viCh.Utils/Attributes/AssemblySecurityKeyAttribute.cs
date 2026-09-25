using System;

namespace P3tr0viCh.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblySecurityKeyAttribute : Attribute
    {
        public string Value { get; }

        public AssemblySecurityKeyAttribute(string value) => Value = value;
    }
}