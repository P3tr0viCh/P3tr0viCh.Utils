using System;
using System.Collections.Generic;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemListEventArgs<T> : EventArgs
    {
        public IEnumerable<T> Values { get; set; } = null;

        public ItemListEventArgs()
        {
        }
    }
}