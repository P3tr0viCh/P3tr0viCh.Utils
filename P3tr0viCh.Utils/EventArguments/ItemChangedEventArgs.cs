using System;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemChangedEventArgs<T> : EventArgs
    {
        public T Value { get; set; } = default;

        public ItemChangedEventArgs()
        {
        }
    }
}