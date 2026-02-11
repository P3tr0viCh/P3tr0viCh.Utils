using System.Collections.Generic;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemsDialogEventArgs<T> : OkEventArgs
    {
        public List<T> Values { get; set; } = null;

        public ItemsDialogEventArgs()
        {
        }

        public ItemsDialogEventArgs(List<T> values) : this()
        {
            Values = values;
        }
    }
}