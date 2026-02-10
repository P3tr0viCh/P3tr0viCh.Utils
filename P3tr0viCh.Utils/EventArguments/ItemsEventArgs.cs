using System;
using System.Collections.Generic;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemsEventArgs<T> : EventArgs
    {
        public IEnumerable<T> Values { get; set; } = null;

        public ItemsEventArgs()
        {
        }

        public ItemsEventArgs(IEnumerable<T> values) : this()
        {
            Values = values;
        }
    }
}