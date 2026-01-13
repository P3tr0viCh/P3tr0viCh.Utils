using P3tr0viCh.Utils.Extensions;
using System;

namespace P3tr0viCh.Utils.Formatters
{
    public class BoolFormatter : ICustomFormatter, IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is bool value)
            {
                if (format.IsEmpty())
                {
                    return value.ToString();
                }

                var words = format.Split('|');

                return value ? words[0] : (words.Length > 1 ? words[1] : string.Empty);
            }

            return string.Empty;
        }
    }
}