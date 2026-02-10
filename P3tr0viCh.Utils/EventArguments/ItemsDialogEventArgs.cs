using System.Collections.Generic;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemsDialogEventArgs<T> : OkEventArgs
    {
        public IEnumerable<T> Values { get; set; } = null;

        public ItemsDialogEventArgs()
        {
        }

        public ItemsDialogEventArgs(IEnumerable<T> values) : this()
        {
            Values = values;
        }
    }
}