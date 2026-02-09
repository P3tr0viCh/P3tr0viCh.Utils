using System;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemEventArgs<T> : EventArgs
    {
        public T Value { get; set; } = default;

        public ItemEventArgs()
        {
        }
    }
}