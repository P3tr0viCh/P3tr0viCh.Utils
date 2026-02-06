using System;
using System.Collections.Generic;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemListChangedEventArgs<T> : EventArgs
    {
        public IEnumerable<T> Values { get; set; } = null;

        public ItemListChangedEventArgs()
        {
        }
    }
}